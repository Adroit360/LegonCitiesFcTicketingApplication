using LegonCitiesFcTicketingPlatform.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LegonCitiesFcTicketingPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Match> Matches { get; set; }

        public DbSet<TicketSale> TicketsSales { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
