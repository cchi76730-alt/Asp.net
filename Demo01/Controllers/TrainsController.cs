using Demo01.Data;
using Demo01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🔥 GET TRIPS (QUAN TRỌNG NHẤT)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var data = await _context.TrainTrips
                .Join(_context.Trains,
                    trip => trip.TrainId,
                    train => train.Id,
                    (trip, train) => new { trip, train })

                .Join(_context.Routes,
                    t => t.trip.RouteId,
                    route => route.Id,
                    (t, route) => new { t.trip, t.train, route })

                // 🔥 JOIN STATION (QUAN TRỌNG NHẤT)
                .Join(_context.Stations,
                    t => t.route.OriginStationId,
                    s => s.Id,
                    (t, originStation) => new { t.trip, t.train, t.route, originStation })

                .Join(_context.Stations,
                    t => t.route.DestinationStationId,
                    s => s.Id,
                    (t, destStation) => new
                    {
                        id = t.trip.Id,
                        trainCode = t.train.TrainCode,

                        // 🔥 FIX CHỖ NÀY
                        from = t.originStation.Name,
                        to = destStation.Name,

                        date = t.trip.DepartureTime,
                        time = t.trip.DepartureTime,
                        price = 100
                    })
                .ToListAsync();

            return Ok(data);
        }

        // =========================
        // GET TRIP BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _context.TrainTrips
                .Include(t => t.Train)
                .Include(t => t.Route)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null) return NotFound();

            return Ok(trip);
        }

        // =========================
        // GET TRIPS BY TRAIN
        // =========================
        [HttpGet("train/{trainId}")]
        public async Task<IActionResult> GetTripsByTrain(int trainId)
        {
            var trips = await _context.TrainTrips
                .Where(t => t.TrainId == trainId) // 🔥 FIX ĐÚNG
                .ToListAsync();

            return Ok(trips);
        }
    }
}