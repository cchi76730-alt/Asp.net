namespace Demo01.Models
{
    public class TrainTrip
    {
        public int Id { get; set; }

        // 🔥 FK
        public int TrainId { get; set; }
        public int RouteId { get; set; }

        public DateTime DepartureTime { get; set; }

        public string Status { get; set; } = "ACTIVE";

        // 🔥 NAVIGATION
        public Train Train { get; set; }
        public TrainRoute Route { get; set; }
    }
}