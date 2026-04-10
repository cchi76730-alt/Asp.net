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
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _context.TrainTrips.ToListAsync();
            return Ok(trips);
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
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] TrainTrip trip)
        {
            _context.TrainTrips.Add(trip);
            await _context.SaveChangesAsync();

            return Ok(trip);
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