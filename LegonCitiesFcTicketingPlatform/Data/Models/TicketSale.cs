using System;

namespace LegonCitiesFcTicketingPlatform.Data.Models
{
    public class TicketSale
    {
        public int Id { get; set; }

        public string BuyerPhone { get; set; }

        public DateTime DateBought { get; set; }

        public string VoucherCode { get; set; }

        public bool IsPaid { get; set; }

        public int PaymentId { get; set; }

        public virtual Ticket Ticket { get; set; }

        public bool IsVerified { get; set; }
    }
}
