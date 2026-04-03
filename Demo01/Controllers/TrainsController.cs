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

        [HttpGet]
        public async Task<IActionResult> GetTrains()
        {
            return Ok(await _context.Trains.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrain(Train train)
        {
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();
            return Ok(train);
        }
    }
}