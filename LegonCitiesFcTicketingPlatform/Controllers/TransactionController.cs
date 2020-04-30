using LegonCitiesFcTicketingPlatform.Data;
using LegonCitiesFcTicketingPlatform.Data.DTO_s;
using LegonCitiesFcTicketingPlatform.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LegonCitiesFcTicketingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        public ApplicationDbContext dbContext { get; set; }

        public AppSettings AppSettings;

        public MessagingService MessagingService { get; set; }
        public TransactionController(ApplicationDbContext _dbcontext, IOptions<AppSettings> _appSettings, MessagingService messagingService)
        {
            dbContext = _dbcontext;
            AppSettings = _appSettings.Value;
            MessagingService = messagingService;

        }
        /// <summary>
        /// Verifies a payment transaction
        /// </summary>
        /// <param name="txRef">The Unique reference of the transaction</param>
        /// <param name="paymentType">The type of payment momo or card</param>
        /// <param name="status">An already known status of the transaction</param>
        /// <returns>An Http Status Code</returns>
        [HttpPost("verifyticketSalePayment/{txRef}")]
        public async Task<IActionResult> VerifyticketSalePayment(string txRef, string status)
        {
            var ticketSale = dbContext.TicketsSales.Where(i => i.VoucherCode == txRef).Include("Ticket").Include("Ticket.Match").FirstOrDefault();
            string saleStatus = null;

            if (ticketSale == null)
            {
                return BadRequest(new { status = "", errorString = "ticketSale stake was not found" });
            }
            else if (ticketSale.IsPaid)
            {
                saleStatus = ticketSale.IsPaid ? "paid" : "not Paid";
                return Ok(new { saleStatus, message = "ticketSale Already Verified" });
            }

            var m = ticketSale.Ticket.Match;
            string errorString = null;

            try
            {
                var VResult = VerifyPayment(ticketSale.PaymentId, status).Result;
                if (VResult.isConfirmed)
                {
                    if (ticketSale.IsPaid)
                        return Ok(new { ticketSale, errorString });

                    ticketSale.IsPaid = true;
                }
                else
                {
                    if (VResult.message == "PENDING" || VResult.message == "PROGRESS" || VResult.message == "NEW")
                    {

                    }
                    else
                        ticketSale.IsPaid = false;
                }
                dbContext.Entry(ticketSale).State = EntityState.Modified;
                dbContext.SaveChanges();

                if (ticketSale.IsPaid) //send the currently added participant to all clients
                {
                    //Send as sms to the user
                    MessagingService.SendSms(ticketSale.BuyerPhone, Misc.GetTicketPurchaseMessage(ticketSale));
                }


            }
            catch (Exception ex)
            {
                errorString = ex.Message;
            }

            saleStatus = ticketSale.IsPaid ? "paid" : "not Paid";
            return Ok(new { saleStatus, errorString });

        }

        async Task<(string message, bool isConfirmed)> VerifyPayment(int? paymentId, string status = null)
        {
            if (status?.ToLower() == "paid")
                return ("PAID", true);
            else if (status != null)
                return (status, false);
            else
            {


                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("apikey", AppSettings.ReddeSettings.ApiKey);
                httpClient.DefaultRequestHeaders.Add("appid", AppSettings.ReddeSettings.AppId);



                string requestUrl = $"https://api.reddeonline.com/v1/status/{paymentId}";


                var req = new HttpRequestMessage();
                req.Content = new StringContent("{}",
                                Encoding.UTF8,
                                "application/json");
                //req.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                req.Method = HttpMethod.Get;
                req.RequestUri = new Uri(requestUrl);



                var responseMessage = await httpClient.SendAsync(req);

                var contentString = await responseMessage.Content.ReadAsStringAsync();

                try
                {
                    ReddeStatusResponse response = JsonConvert.DeserializeObject<ReddeStatusResponse>(contentString);

                    if (response.Status.ToLower() == "paid")
                        return ("PAID", true);
                    else return (response.Status, false);
                }
                catch
                {
                    return (null, false);
                }

            }
        }

        [HttpPost("redderecievehook")]
        public async Task<IActionResult> ReddeRecieveWebHook(ReddeRecieveWebhookCallback response)
        {
            await VerifyticketSalePayment(response.Clienttransid, response.Status);
            Console.WriteLine(response.Clienttransid + " " + response.Status + " " + response.Reason);
            return Ok();
        }


        [HttpGet("getrecordswithvoucher/{voucher}")]
        public async Task<IActionResult> GetRecordWithVoucher(string voucher)
        {
            var ticketSale = dbContext.TicketsSales.Where(i => i.VoucherCode == voucher).Include("Ticket").Include("Ticket.Match").FirstOrDefault();

            if (ticketSale != null)
            {
                if (ticketSale.IsPaid)
                {
                    var isVerified = ticketSale.IsVerified;
                    ticketSale.IsVerified = true;
                    dbContext.Entry(ticketSale).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        phone = ticketSale.BuyerPhone,
                        category = ticketSale.Ticket.TicketType.ToString(),
                        match = ticketSale.Ticket.Match.Name,
                        isVerified = isVerified
                    });

                }
                else
                {
                    return BadRequest(new { error = "This ticket has not been paid for" });
                }
            }
            else
            {
                return BadRequest(new { error = "Incorrect Ticket Number" });
            }

        }
    }
}