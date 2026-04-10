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
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetTrains()
        {
            return Ok(await _context.Trains.ToListAsync());
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrain(int id)
        {
            var train = await _context.Trains.FindAsync(id);
            if (train == null) return NotFound();

            return Ok(train);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateTrain([FromBody] Train train)
        {
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();

            return Ok(train);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrain(int id, Train train)
        {
            if (id != train.Id) return BadRequest();

            var existing = await _context.Trains.FindAsync(id);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(train);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var train = await _context.Trains.FindAsync(id);
            if (train == null) return NotFound();

            _context.Trains.Remove(train);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // GET TRIPS BY TRAIN
        // =========================
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTripsByTrain(int id)
        {
            var trips = await _context.TrainTrips
                .Where(t => t.TrainId == id)
                .ToListAsync();

            return Ok(trips);
        }
    }
}