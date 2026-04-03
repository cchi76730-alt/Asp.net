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

        // Xem ghế theo trip
        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetSeatsByTrip(int tripId)
        {
            var seats = await _context.TripSeatInventories
                .Where(x => x.TripId == tripId)
                .ToListAsync();

            return Ok(seats);
        }
    }
}