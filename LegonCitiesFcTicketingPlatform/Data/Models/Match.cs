using System;
using System.Collections.Generic;

namespace LegonCitiesFcTicketingPlatform.Data.Models
{
    public class Match
    {
        public Match()
        {
            Tickets = new List<Ticket>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Season { get; set; }

        public DateTime MatchDate { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
    }
}
