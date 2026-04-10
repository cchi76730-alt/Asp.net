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

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetStations()
        {
            var stations = await _context.Stations.ToListAsync();
            return Ok(stations);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
                return NotFound("Không tìm thấy station");

            return Ok(station);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateStation([FromBody] Station station)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Stations.Add(station);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStation), new { id = station.Id }, station);
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] Station station)
        {
            if (id != station.Id)
                return BadRequest("Id không khớp");

            var existing = await _context.Stations.FindAsync(id);
            if (existing == null)
                return NotFound("Không tìm thấy station");

            // 👉 KHÔNG dùng Modified toàn bộ để tránh overwrite bừa
            // 👉 Map thủ công theo field thực tế của bạn

            // Ví dụ nếu Station có Name:
            // existing.Name = station.Name;

            // Nếu bạn chưa rõ field → giữ nguyên cách cũ:
            _context.Entry(existing).CurrentValues.SetValues(station);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
                return NotFound("Không tìm thấy station");

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}