namespace Demo01.Models
{
    public class TripSeatInventory
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int SeatId { get; set; }

        // AVAILABLE / HELD / SOLD
        public string Status { get; set; } = string.Empty;
    }
}