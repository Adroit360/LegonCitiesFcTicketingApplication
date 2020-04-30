using System.Collections.Generic;

namespace LegonCitiesFcTicketingPlatform.Data.Models
{
    public class Ticket
    {
        public Ticket()
        {
            TicketSales = new List<TicketSale>();
        }
        public int Id { get; set; }


        public decimal Price { get; set; }

        /// <summary>
        /// Type of ticket i.e PopularStand.vip or vvip
        /// </summary>
        public TicketType TicketType { get; set; }


        public virtual Match Match { get; set; }

        public virtual List<TicketSale> TicketSales { get; set; }
    }
}
