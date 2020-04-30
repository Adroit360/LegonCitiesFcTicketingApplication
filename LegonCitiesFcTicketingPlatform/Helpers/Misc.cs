

using LegonCitiesFcTicketingPlatform.Data.DTO_s;
using LegonCitiesFcTicketingPlatform.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OtpNet;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegonCitiesFcTicketingPlatform.Helpers
{
    public static class Misc
    {
        public static JsonSerializerSettings getDefaultResolverJsonSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
        }

        /// <summary>
        /// Returns the The Phone Number without the Country Code
        /// </summary>
        /// <param name="phoneNumber">The phoneNumber to normalize</param>
        /// <returns></returns>
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber == null)
                    return phoneNumber;

                if (phoneNumber.Length < 9)
                    return phoneNumber;

                if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
                {
                    return phoneNumber.Substring(1);
                }
                else if (!phoneNumber.StartsWith("0") && phoneNumber.Length == 9)
                {
                    return phoneNumber;
                }
                else if (phoneNumber.StartsWith("233") && phoneNumber.Length == 12)
                {
                    return phoneNumber.Substring(3);
                }

                return phoneNumber.Substring(4);
            }
            catch
            {
                return phoneNumber;
            }

        }

        /// <summary>
        /// Returns the Ghanaian Phone Number in InternationalSyntax
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string FormatGhanaianPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
            {
                return "+233" + phoneNumber.Substring(1);
            }
            else if (!phoneNumber.StartsWith("0") && phoneNumber.Length == 9)
            {
                return "+233" + phoneNumber;
            }
            else if (phoneNumber.StartsWith("233") && phoneNumber.Length == 12)
            {
                return "+" + phoneNumber;
            }

            return phoneNumber;
        }

        public static string FormatGhanaianPhoneNumberWp(string phoneNumber)
        {
            return FormatGhanaianPhoneNumber(phoneNumber).Substring(1);
        }


        public static string getTxRef(string phoneNumber)
        {
            var match = Regex.Match(phoneNumber, @"^(\w{2}).*(\w{2})$");

            var userCode = match.Groups[1].ToString() + match.Groups[2].ToString();
            var timeStamp = DateTime.Now.TimeOfDay.ToString();
            return $"inv.{ userCode}.{timeStamp}";
        }

        public static string getReddePayOption(string phoneNumber)
        {
            var phone = Misc.FormatGhanaianPhoneNumber(phoneNumber);
            var networkDeterminants = phone.Substring(5, 1);
            if (networkDeterminants == "4" || networkDeterminants == "5" || networkDeterminants == "9")
                return "MTN";
            else if (networkDeterminants == "0")
                return "VODAFONE";
            else if (networkDeterminants == "6" || networkDeterminants == "7")
                return "AIRTELTIGO";

            return null;
        }


        public static async Task<int?> GenerateAndSendReddeInvoice(TicketSale ticketSale, ReddeSettingsDTO reddeSettings)
        {
            try
            {

                ReddeRequest request = new ReddeRequest
                {
                    Amount = ticketSale.Ticket.Price,
                    Appid = reddeSettings.AppId,
                    Clientreference = ticketSale.BuyerPhone,
                    Clienttransid = ticketSale.VoucherCode,
                    Description = "Ticket Sale for " + ticketSale.DateBought,
                    Nickname = reddeSettings.NickName,
                    Paymentoption = getReddePayOption(ticketSale.BuyerPhone),
                    Walletnumber = FormatGhanaianPhoneNumberWp(ticketSale.BuyerPhone)
                };

                var httpClient = new HttpClient();


                var data = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var stringContent = new StringContent(data);
                httpClient.DefaultRequestHeaders.Add("apikey", reddeSettings.ApiKey);
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var responseMessage = await httpClient.PostAsync("https://api.reddeonline.com/v1/receive", stringContent);

                var contentString = await responseMessage.Content.ReadAsStringAsync();
                ReddeInitialResponse response = JsonConvert.DeserializeObject<ReddeInitialResponse>(contentString);

                return response.Transactionid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GenerateReddeToken {ex.Message}");
                return null;
            }

        }


        public static TicketType GetTicketType(string ticketTypeNumber)
        {
            if (ticketTypeNumber == "1")
                return TicketType.Popularstand;
            if (ticketTypeNumber == "2")
                return TicketType.Centerline;
            if (ticketTypeNumber == "3")
                return TicketType.Vip;
            if (ticketTypeNumber == "4")
                return TicketType.Vvip;

            return 0;
        }

        public static string GenerateVoucherCode(string userNumber)
        {
            var secretKey = Encoding.UTF8.GetBytes(userNumber);
            var totp = new Totp(secretKey);
            return totp.ComputeTotp();
        }



        public static string GetTicketPurchaseMessage(TicketSale ticketSale)
        {
            return $"Hello {ticketSale.BuyerPhone}, your ticket Number for {ticketSale.Ticket.Match.Name} bought on {ticketSale.DateBought} is {ticketSale.VoucherCode}";
        }

    }
}
