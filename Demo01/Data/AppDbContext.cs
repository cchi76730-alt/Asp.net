using Demo01.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Sockets;
using static System.Collections.Specialized.BitVector32;

namespace Demo01.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Carriage> Carriages { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<TrainRoute> Routes { get; set; }
        public DbSet<TrainTrip> TrainTrips { get; set; }
        public DbSet<TripSeatInventory> TripSeatInventories { get; set; }
        public DbSet<SeatHold> SeatHolds { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPassenger> BookingPassengers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE (trip_id, seat_id)
            modelBuilder.Entity<TripSeatInventory>()
                .HasIndex(t => new { t.TripId, t.SeatId })
                .IsUnique();
        }
    }
}