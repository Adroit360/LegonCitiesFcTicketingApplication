using DalSoft.Hosting.BackgroundQueue;
using LegonCitiesFcTicketingPlatform.Data;
using LegonCitiesFcTicketingPlatform.Data.Models;
using LegonCitiesFcTicketingPlatform.Helpers;
using LegonCitiesFcTicketingPlatform.HostedServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegonCitiesFcTicketingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UssdController : ControllerBase
    {
        Dictionary<string, UssdResponse> ussdResponses = null;
        Dictionary<string, UssdResponse> legonCitiesResponses = null;

        public ApplicationDbContext dbContext { get; set; }
        public AppSettings Settings { get; set; }
        public BackgroundQueue BackgroundQueue { get; set; }
        public ReddePaymentHostedService hostedService { get; set; }

        //[BindProperty]
        //public UssdRequest ussdRequest { get; set; }

        public UssdController(ApplicationDbContext _dbContext, IOptions<AppSettings> _appSettings, BackgroundQueue backgroundQueue, ReddePaymentHostedService _hostedService)
        {
            dbContext = _dbContext;
            Settings = _appSettings.Value;
            BackgroundQueue = backgroundQueue;
            hostedService = _hostedService;

            legonCitiesResponses = new Dictionary<string, UssdResponse>()
            {
                #region Level 0
                {
                    "null",new UssdResponse{
                        Message = "Legon Cities FC Ticketing Platform\nDiscover fixtures and be a part of our club\n1. Buy a ticket\n2. Check fixtures\n3. Contact Us",
                        Type="Response",
                        ClientState = "0"
                    }
                },
                #endregion

                #region Level 1
                {
                    "1",new UssdResponse
                    {
                        Message = "Which type of ticket\n1. Popular stand\n2.Center line\n3. VIP\n4. VVIP",
                        Type= "Response",
                        ClientState = "1"
                    }

                },
                {
                    "2",new UssdResponse
                    {
                        Message = "",
                        Type = "Release",
                        ClientState = "2.0"
                    }
                },
                {
                    "3",new UssdResponse
                    {
                        Message = "Contact Information\nEmail : info@legoncitiesfc.com\nPhone/Whatsapp : 0302216549",
                        Type = "Release",
                        ClientState = "4"
                    }
                },
                #endregion

                #region Level 2
                {
                    "1-1",new UssdResponse
                    {
                        Message="\n1. Buy\n0. Back",
                        Type = "Response",
                        ClientState = "1-1"

                    }
                },
                {
                    "1-2",new UssdResponse
                    {
                        Message= "\n1. Buy\n0. Back",
                        Type = "Response",
                        ClientState = "1-2"

                    }
                },
                {
                    "1-3",new UssdResponse
                    {
                         Message="\n1. Buy\n0. Back",
                        Type = "Response",
                        ClientState = "1-3"

                    }
                }
                #endregion

                ,
                {
                    "Summary",new UssdResponse
                    {
                         Message="\n1. Proceed\n0. Go Back",
                        Type = "Response",
                        ClientState = "Dynamic Data"

                    }
                }
                ,
                {
                    "Thankyou",new UssdResponse
                    {
                         Message="Thank you for buying your ticket, Wait for the momo prompt.\n You'll recive your ticket details after completing the process",
                        Type = "Release",
                        ClientState = "Dynamic Data"

                    }
                }

            };

            ussdResponses = new Dictionary<string, UssdResponse>()
            {
                #region Level 0
                {
                    "null",new UssdResponse{
                        Message = "Welcome to Best Assurance\n1. Buy Insurance\n2. Renew Insurance\n3. Re-enact Failed Insurance\n4. Make a Report\n5.Check Insurance Status",
                        Type="Response",
                        ClientState = "0"
                    }
                },
                #endregion

                #region Level 1
                {
                    "1",new UssdResponse
                    {
                        Message = "Select an option\n1. Motor Insurance\n2. Travel Insurance",
                        Type= "Response",
                        ClientState = "1"
                    }

                },
                {
                    "2",new UssdResponse
                    {
                        Message = "Select an option\n1. Motor Insurance",
                        Type = "Response",
                        ClientState = "2"
                    }
                },
                {
                    "3",new UssdResponse
                    {
                        Message = "Please enter the order Id for the transaction.",
                        Type = "Response",
                        ClientState = "3"
                    }
                },
                {
                    "4",new UssdResponse
                    {
                        Message = "Please enter your report message , do well to keep it short",
                        Type = "Response",
                        ClientState = "3"
                    }
                },
                {
                    "5",new UssdResponse
                    {
                        Message = "Please enter your insurance number",
                        Type = "Response",
                        ClientState = "3"
                    }
                },
                #endregion

                #region Level 2
                {
                    "1-1",new UssdResponse
                    {
                        Message="\n1. 3rd Party\n2. 3rd Party,Fire and Theft\n3. Comprehensive\n0. Main Menu",
                        Type = "Response",
                        ClientState = "1-1"

                    }
                },
                {
                    "1-2",new UssdResponse
                    {
                        Message= "\n1. Individual\n2. Agent\n3. Broker\n0. Main Menu",
                        Type = "Response",
                        ClientState = "1-2"

                    }
                },
                {
                    "2-1",new UssdResponse
                    {
                        Message= "\n1. Individual\n2. Agent\n3. Broker\n0. Main Menu",
                        Type = "Response",
                        ClientState = "2-1"

                    }
                },
                {
                    "3-1",new UssdResponse
                    {
                        Message= "Wrong input. Please enter the correct Order Id",
                        Type = "Response",
                        ClientState = "3"

                    }
                },
                {
                    "4-1",new UssdResponse
                    {
                        Message= "Thank you for the report. We will review and tackle it",
                        Type = "Release",
                        ClientState = "4-1"

                    }
                },
                {
                    "5-1",new UssdResponse
                    {
                        Message= "Motor Insurance\nMoto Status : Good\nInsurance Expiry : In 3 months\n",
                        Type = "Release",
                        ClientState = "5-1"

                    }
                },
                #endregion

                #region Level 3
                {
                    "1-1-1",new UssdResponse
                    {
                        Message="\n1. Individual\n2. Agent\n3. Broker\n0. Back To Menu",
                        Type = "Response",
                        ClientState = "1-1-1"

                    }
                },
                {
                    "1-1-2",new UssdResponse
                    {
                        Message="\n1. Individual\n2. Agent\n3. Broker\n0. Back To Menu",
                        Type = "Response",
                        ClientState = "1-1-2"

                    }
                },
                {
                    "1-1-3",new UssdResponse
                    {
                         Message="\n1. Individual\n2. Agent\n3. Broker\n0. Back To Menu",
                        Type = "Response",
                        ClientState = "1-1-3"

                    }
                },
                {
                    "1-2-1",new UssdResponse
                    {
                        Message="\n1. Please enter the number of days you wish to spend on the trip",
                        Type = "Response",
                        ClientState = "1-2-1"

                    }
                },
                {
                    "1-2-2",new UssdResponse
                    {
                        Message="Please enter your agency code",
                        Type = "Response",
                        ClientState = "1-2-2"

                    }
                },
                {
                    "1-2-3",new UssdResponse
                    {
                         Message="Please enter your broker code",
                        Type = "Response",
                        ClientState = "1-2-3"

                    }
                },
                {
                    "2-1-1",new UssdResponse
                    {
                        Message="\n1. Please enter the number of days you wish to spend on the trip",
                        Type = "Response",
                        ClientState = "2-1-1"

                    }
                },
                {
                    "2-1-2",new UssdResponse
                    {
                        Message="Please enter your agency code",
                        Type = "Response",
                        ClientState = "2-1-2"

                    }
                },
                {
                    "2-1-3",new UssdResponse
                    {
                         Message="Please enter your broker code",
                        Type = "Response",
                        ClientState = "2-1-3"

                    }
                },

                #endregion

                #region level 4
                {
                    "1-1-1-1",new UssdResponse
                    {
                        Message="Please enter your vehicle Reg Number \n(Eg. GT 1234-19 or GT 1234 Z)",
                        Type = "Response",
                        ClientState = "1-1-1-1"

                    }
                },
                {
                    "1-1-1-2",new UssdResponse
                    {
                        Message="Enter your agency code",
                        Type = "Response",
                        ClientState = "1-1-1-2"

                    }
                },
                {
                    "1-1-1-3",new UssdResponse
                    {
                        Message="Enter your broker code",
                        Type = "Response",
                        ClientState = "1-1-1-3"

                    }
                },


                {
                    "1-1-2-1",new UssdResponse
                    {
                        Message="Please enter your vehicle Reg Number \n(Eg. GT 1234-19 or GT 1234 Z)",
                        Type = "Response",
                        ClientState = "1-1-2-1"

                    }
                },
                {
                    "1-1-2-2",new UssdResponse
                    {
                        Message="Enter your agency code",
                        Type = "Response",
                        ClientState = "1-1-2-2"

                    }
                },
                {
                    "1-1-2-3",new UssdResponse
                    {
                        Message="Enter your broker code",
                        Type = "Response",
                        ClientState = "1-1-2-3"

                    }
                },


                {
                    "1-1-3-1",new UssdResponse
                    {
                        Message="Please enter your vehicle Reg Number \n(Eg. GT 1234-19 or GT 1234 Z)",
                        Type = "Response",
                        ClientState = "1-1-3-1"

                    }
                },
                {
                    "1-1-3-2",new UssdResponse
                    {
                        Message="Enter your agency code",
                        Type = "Response",
                        ClientState = "1-1-3-2"

                    }
                },
                {
                    "1-1-3-3",new UssdResponse
                    {
                        Message="Enter your broker code",
                        Type = "Response",
                        ClientState = "1-1-3-3"

                    }
                },


                {
                    "1-2-1-1",new UssdResponse
                    {
                        Message="Please enter your vehicle Reg Number \n(Eg. GT 1234-19 or GT 1234 Z)",
                        Type = "Response",
                        ClientState = "1-1-3-1"

                    }
                },


                #endregion
                {
                    "Summary",new UssdResponse
                    {
                         Message="\n1. Proceed\n0. Go Back",
                        Type = "Response",
                        ClientState = "Dynamic Data"

                    }
                }
                ,
                {
                    "Thankyou",new UssdResponse
                    {
                         Message="Thank you for buying your ticket, Wait for the momo prompt.\n You'll recive your ticket details after completing the process",
                        Type = "Release",
                        ClientState = "Dynamic Data"

                    }
                }

            };

        }


        [HttpGet("index")]
        public IActionResult First()
        {
            return Content("It is working");
        }

        [HttpPost("app")]
        public async Task<IActionResult> BestAssurance([FromBody]UssdRequest ussdRequest)
        {
            try
            {

                if (ussdRequest.Sequence == "1")
                {
                    return Json(ussdResponses["null"]);
                }
                else
                {
                    try
                    {
                        UssdResponse response = new UssdResponse();
                        if (ussdRequest.ClientState == "0")
                            response = ussdResponses[$"{ussdRequest.Message}"];
                        else
                            response = ussdResponses[$"{ussdRequest.ClientState}-{ussdRequest.Message}"];

                        return Json(response);
                    }
                    catch
                    {
                        var response = ussdResponses["null"];
                        response.Message = "Invalid Input\n" + response.Message;
                        return Json(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost("legoncities")]
        public async Task<IActionResult> LegonCities([FromBody]UssdRequest ussdRequest)
        {
            try
            {
                switch (ussdRequest.ClientState)
                {
                    case null:
                        return Json(ussdResponses["null"]);
                    case "0":
                        try
                        {
                            var response0 = ussdResponses[$"{ussdRequest.Message}"];
                            return Json(response0);
                        }
                        catch
                        {
                            var response = ussdResponses["null"];
                            response.Message = "Invalid Input\n" + response.Message;
                            return Json(response);
                        }
                    case "1":
                        try
                        {
                            var ussdResponse = ussdResponses[$"1-{ussdRequest.Message}"];
                            ussdResponse.ClientState = $"1-{ussdRequest.Message}";
                            var response1 = ussdResponses[$"1-{ussdRequest.Message}"];
                            response1.Message = GetMatchDataString(ussdRequest.Message) + response1.Message;
                            return Json(response1);
                        }
                        catch
                        {
                            var response = ussdResponses["1"];
                            response.Message = "Invalid Input\n" + response.Message;
                            return Json(response);
                        }
                    case var second when new Regex(@"^2").IsMatch(second):
                        try
                        {
                            var response2 = ussdResponses[$"2"];
                            response2.Message = GetFixtureDataString();
                            //Set the client state base on the current page of the fixture data retrieved
                            var page = second.Substring(2, 1);
                            response2.ClientState = "2-0";
                            return Json(response2);
                        }
                        catch
                        {
                            var response = ussdResponses["2"];
                            response.Message = "Invalid Input\n" + response.Message;
                            return Json(response);
                        }

                    case "3":
                        try
                        {
                            return Json(ussdResponses["4"]);
                        }
                        catch (Exception)
                        {

                            throw;
                        }


                    case var ticketTypeLevel when new Regex(@"^1-\d+$").IsMatch(ticketTypeLevel):
                        try
                        {
                            //Show the purchase Summary
                            var requestMessage = ussdRequest.Message;
                            if (requestMessage == "1")
                            {
                                var clientStateRegex = new Regex(@"^(?<unwanted>\d+)-(?<tickettypenumber>\d+)$").Match(ussdRequest.ClientState);
                                var TicketTypeNumber = clientStateRegex.Groups["tickettypenumber"].ToString();

                                var response = ussdResponses["Summary"];
                                response.Message = GetPurchaseSummary(TicketTypeNumber, ussdRequest.Mobile) + response.Message;

                                response.ClientState = "*" + ussdRequest.ClientState;

                                return Json(response);
                            }
                            else if (requestMessage == "0")
                            {
                                //User wants to Go Back
                            }

                            return Json(ussdResponses["1-1"]);

                        }
                        catch
                        {
                            var response = ussdResponses["2"];
                            response.Message = "Invalid Input\n" + response.Message;
                            return Json(response);
                        }


                    case var summaryLevel when new Regex(@"^\*1-\d+$").IsMatch(summaryLevel):
                        try
                        {
                            var clientStateRegex = new Regex(@"^\*(?<unwanted>\d+)-(?<tickettypenumber>\d+)$").Match(ussdRequest.ClientState);
                            var TicketTypeNumber = clientStateRegex.Groups["tickettypenumber"].ToString();

                            BackgroundQueue.Enqueue(async cancellationToken =>
                            {
                                hostedService.DoWork(TicketTypeNumber, ussdRequest.Mobile);
                            });

                            return Json(ussdResponses["Thankyou"]);
                        }
                        catch
                        {
                            return Json(ussdResponses["1"]);
                        }
                    default:
                        return Json(ussdResponses["null"]);
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }



        string GetPurchaseSummary(string ticketTypeNumber, string userNumber)
        {
            TicketType ticketType = Misc.GetTicketType(ticketTypeNumber);

            var mobileNumber = "0" + Misc.NormalizePhoneNumber(userNumber);


            var ticket = dbContext.Tickets.Where(t => t.Match.MatchDate >= DateTime.Now && t.TicketType == ticketType).Include("Match").OrderBy(i => i.Match.MatchDate).FirstOrDefault();

            return ticket.Match.Name + "\n" + ticketType.ToString() + "\n" + ticket.Price + " Cedis";

            //return "Legon Cities Fc Vs Hearts - 2/2/20:19:00 = 90 Cedis";

        }


        string GetMatchDataString(string ticketTypeNumber)
        {
            TicketType ticketType = Misc.GetTicketType(ticketTypeNumber);

            var ticket = dbContext.Tickets.Where(t => t.Match.MatchDate >= DateTime.Now && t.TicketType == ticketType).Include("Match").OrderBy(i => i.Match.MatchDate).FirstOrDefault();

            var matchString = ticket.Match.Name + " - " + ticket.Match.MatchDate.ToShortDateString() + " = " + ticket.Price + " Cedis";

            return matchString;
        }

        string GetFixtureDataString()
        {

            var matches = dbContext.Matches.Where(t => t.MatchDate >= DateTime.Now).OrderBy(i => i.MatchDate);

            string matchString = "";

            foreach (var match in matches)
            {
                matchString += match.Name + " - " + match.MatchDate.ToShortDateString() + "\n";
            }


            return matchString;
        }



        JsonResult Json(object result)
        {
            return new JsonResult(result, Misc.getDefaultResolverJsonSettings());
        }

    }

    public class UssdRequest
    {
        public string Type { get; set; }

        public string Mobile { get; set; }

        public string SessionId { get; set; }

        public string ServiceCode { get; set; }

        public string Message { get; set; }

        public string Operator { get; set; }

        public string Sequence { get; set; }

        public string ClientState { get; set; }


    }

    public class UssdResponse
    {
        public string Message { get; set; }

        public string Type { get; set; }

        public string ClientState { get; set; }
    }
}