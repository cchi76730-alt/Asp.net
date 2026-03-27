namespace Demo01.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int TripId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public DateTime BookingDate { get; set; }
    }
}