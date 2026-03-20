namespace Demo01.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int CarriageId { get; set; }
        public string SeatNo { get; set; } = string.Empty;

        public Carriage? Carriage { get; set; }
    }
}