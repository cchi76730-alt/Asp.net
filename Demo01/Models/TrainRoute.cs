using Demo01.Models;

public class TrainRoute
{
    public int Id { get; set; }

    public int OriginStationId { get; set; }
    public int DestinationStationId { get; set; }

    public Station OriginStation { get; set; }   // 🔥 bắt buộc
    public Station DestinationStation { get; set; } // 🔥 bắt buộc
}