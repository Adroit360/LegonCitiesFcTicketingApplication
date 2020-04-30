using LegonCitiesFcTicketingPlatform.Data;
using LegonCitiesFcTicketingPlatform.Data.Models;
using LegonCitiesFcTicketingPlatform.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegonCitiesFcTicketingPlatform.HostedServices
{
    public interface IScopedProcessingService : IDisposable
    {
        void DoWork(string ticketTypeNumber, string userNumber);
    }

    public class ScopedProcessingService : IScopedProcessingService
    {
        public ApplicationDbContext dbContext { get; set; }
        public AppSettings Settings { get; set; }
        public IServiceScopeFactory serviceFactory { get; set; }

        public ScopedProcessingService(IOptions<AppSettings> _appSettings, IServiceScopeFactory _serviceFactory)
        {
            Settings = _appSettings.Value;
            serviceFactory = _serviceFactory;
        }

        public void DoWork(string ticketTypeNumber, string userNumber)
        {
            using (var scope = serviceFactory.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                PersistReddeUssdData(ticketTypeNumber, userNumber);
            }

        }

        public void Dispose()
        {
        }

        async Task PersistReddeUssdData(string ticketTypeNumber, string userNumber)
        {

            try
            {
                TicketType ticketType = Misc.GetTicketType(ticketTypeNumber);


                var mobileNumber = "0" + Misc.NormalizePhoneNumber(userNumber);

                //voucher code is same as transaction Reference
                var txRef = Misc.GenerateVoucherCode(mobileNumber);

                var ticket = dbContext.Tickets.Where(t => t.Match.MatchDate >= DateTime.Now && t.TicketType == ticketType).OrderBy(i => i.Match.MatchDate).FirstOrDefault();
                TicketSale ticketSale = null;
                if (ticket != null)
                {
                    ticketSale = new TicketSale
                    {
                        BuyerPhone = Misc.FormatGhanaianPhoneNumberWp(mobileNumber),
                        DateBought = DateTime.Now,
                        Ticket = ticket,
                        VoucherCode = txRef
                    };
                    dbContext.TicketsSales.Add(ticketSale);
                }
                else
                {
                    //Take an Action if the ticket is not found
                }


                dbContext.SaveChanges();

                try
                {
                    var transactionId = Misc.GenerateAndSendReddeInvoice(ticketSale, Settings.ReddeSettings).Result;
                    ticketSale.PaymentId = (int)transactionId;
                    dbContext.Entry(ticketSale).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    dbContext.SaveChanges();

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {

            }
        }


    }

    public class ReddePaymentHostedService : IDisposable
    {
        public IServiceProvider serviceProvider { get; set; }

        public ReddePaymentHostedService(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void DoWork(string ticketTypeNumber, string userNumber)
        {
            using (var scopedService = serviceProvider.GetRequiredService<IScopedProcessingService>())
            {
                scopedService.DoWork(ticketTypeNumber, userNumber);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
