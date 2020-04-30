using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LegonCitiesFcTicketingPlatform.Helpers
{
    public class MessagingService
    {

        public MessagingService(IOptions<AppSettings> appSettings)
        {
            AppSetting = appSettings.Value;
        }

        public AppSettings AppSetting { get; }
        public async Task SendSms(string phoneNumber, string message, string senderId = "LCitiesFc")
        {
            //return;

            if (string.IsNullOrEmpty(phoneNumber))
                return;

            SendMNotifySms(phoneNumber, message, senderId);
        }

        public void SendMNotifySms(string phoneNumber, string message, string senderId)
        {
            phoneNumber = Misc.FormatGhanaianPhoneNumber(phoneNumber);
            string url = $"https://apps.mnotify.net/smsapi?key={AppSetting.MNotifySettings.ApiKey}&to={phoneNumber}&msg={message}&sender_id={senderId}";


            var httpClient = new HttpClient();

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(url).Result;

                string m = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
