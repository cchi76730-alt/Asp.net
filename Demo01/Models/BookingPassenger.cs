namespace Demo01.Models
{
    public class BookingPassenger
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}