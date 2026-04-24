namespace Demo01.Models
{
    public class SeatHold
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int SeatId { get; set; }

        public string Status { get; set; } = "ACTIVE";

        // ❌ KHÔNG dùng ?
        public DateTime ExpiredAt { get; set; }

        public int CustomerId { get; set; }
    }
}