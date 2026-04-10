using Demo01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeatsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // ALL SEATS BY TRIP
        // =========================
        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetSeatsByTrip(int tripId)
        {
            var seats = await _context.TripSeatInventories
                .Where(x => x.TripId == tripId)
                .Select(x => new
                {
                    x.SeatId,
                    x.Status
                })
                .ToListAsync();

            return Ok(seats);
        }

        // =========================
        // AVAILABLE SEATS
        // =========================
        [HttpGet("trip/{tripId}/available")]
        public async Task<IActionResult> GetAvailableSeats(int tripId)
        {
            var seats = await _context.TripSeatInventories
                .Where(x => x.TripId == tripId && x.Status == "AVAILABLE")
                .Select(x => new
                {
                    x.SeatId,
                    x.Status
                })
                .ToListAsync();

            return Ok(seats);
        }
    }
}