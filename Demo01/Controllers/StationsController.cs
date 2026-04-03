using Microsoft.AspNetCore.Mvc;
using Demo01.Data;
using Demo01.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/stations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
            return await _context.Stations.ToListAsync();
        }

        // GET: api/stations/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Station>> GetStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
                return NotFound();

            return station;
        }

        // POST: api/stations
        [HttpPost]
        public async Task<ActionResult<Station>> CreateStation(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStation), new { id = station.Id }, station);
        }

        // PUT: api/stations/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, Station station)
        {
            if (id != station.Id)
                return BadRequest();

            _context.Entry(station).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/stations/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null)
                return NotFound();

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}