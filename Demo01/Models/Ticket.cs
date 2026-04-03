namespace Demo01.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int TripId { get; set; }
        public int SeatId { get; set; }

        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;

        // Navigation
        public Booking Booking { get; set; }
    }
}