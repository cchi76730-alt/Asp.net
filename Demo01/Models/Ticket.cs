namespace Demo01.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int TripId { get; set; }
        public int SeatId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}