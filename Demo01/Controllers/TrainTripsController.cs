using Demo01.Data;
using Demo01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainTripsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainTripsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL (FIXED CLEAN)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var data = await _context.TrainTrips
                .Include(t => t.Train)
                .Include(t => t.Route)
                    .ThenInclude(r => r.OriginStation)
                .Include(t => t.Route)
                    .ThenInclude(r => r.DestinationStation)
                .Select(t => new
                {
                    id = t.Id,
                    trainCode = t.Train.TrainCode,

                    // 🔥 FIX CHỖ NÀY
                    from = t.Route.OriginStation.Name,
                    to = t.Route.DestinationStation.Name,

                    date = t.DepartureTime,
                    time = t.DepartureTime,
                    price = 100
                })
                .ToListAsync();

            return Ok(data);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _context.TrainTrips.FindAsync(id);
            if (trip == null) return NotFound();

            return Ok(trip);
        }

        // =========================
        // SEARCH
        // =========================
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips([FromQuery] int routeId)
        {
            var trips = await _context.TrainTrips
                .Where(t => t.RouteId == routeId && t.Status == "OPEN")
                .ToListAsync();

            return Ok(trips);
        }

        // =========================
        // CREATE TRIP + AUTO SEATS
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateTrip(TrainTrip trip)
        {
            _context.TrainTrips.Add(trip);
            await _context.SaveChangesAsync();

            var seats = new List<TripSeatInventory>();

            for (int i = 1; i <= 40; i++)
            {
                seats.Add(new TripSeatInventory
                {
                    TripId = trip.Id,
                    SeatId = i,
                    Status = "AVAILABLE"
                });
            }

            _context.TripSeatInventories.AddRange(seats);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Trip + seats created",
                tripId = trip.Id,
                seatCount = 40
            });
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, TrainTrip trip)
        {
            if (id != trip.Id) return BadRequest();

            var existing = await _context.TrainTrips.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var trip = await _context.TrainTrips.FindAsync(id);
            if (trip == null) return NotFound();

            _context.TrainTrips.Remove(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}