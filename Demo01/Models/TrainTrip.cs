using System;

namespace Demo01.Models
{
    public class TrainTrip
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}