namespace Demo01.Models
{
    public class Train
    {
        public int Id { get; set; }
        public string TrainCode { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int Price { get; set; }
    }
}