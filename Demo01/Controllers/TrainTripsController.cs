using Demo01.Data;
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

        // Tìm chuyến
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips(int routeId)
        {
            var trips = await _context.TrainTrips
                .Where(t => t.RouteId == routeId && t.Status == "OPEN")
                .ToListAsync();

            return Ok(trips);
        }
    }
}