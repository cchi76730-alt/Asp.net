namespace Demo01.Models
{
    public class Carriage
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int CarriageNo { get; set; }

        public Train? Train { get; set; }
    }
}