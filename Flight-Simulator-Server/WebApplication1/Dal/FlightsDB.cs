using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Dal
{
    public class FlightsDB : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Leg> Legs { get; set; }

        public FlightsDB(DbContextOptions<FlightsDB> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LegStation>()
        .HasKey(ls => ls.Id);

            modelBuilder.Entity<LegStation>()
                .HasOne(ls => ls.MultyLeg)
                .WithOne()
                .HasForeignKey<LegStation>(ls => ls.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LegStation>()
                .HasOne(ls => ls.SubLeg)
                .WithOne()
                .HasForeignKey<LegStation>(ls => ls.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
