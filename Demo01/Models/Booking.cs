namespace Demo01.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string BookingCode { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int TripId { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = "PENDING_PAYMENT";

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;

        // ✅ FIX: thêm navigation list
        public List<Ticket>? Tickets { get; set; }

        public int SeatId { get; set; } // 🔥 THÊM DÒNG NÀY
    }
}