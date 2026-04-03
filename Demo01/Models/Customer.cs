namespace Demo01.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<Booking> Bookings { get; set; }
    }
}