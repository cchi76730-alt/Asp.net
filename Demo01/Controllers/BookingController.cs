using Demo01.Data;
using Demo01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        public class HoldSeatRequest
        {
            public int TripId { get; set; }
            public int SeatId { get; set; }
        }

        public class BookSeatRequest
        {
            public int TripId { get; set; }
            public int SeatId { get; set; }
            public int CustomerId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tickets)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return Ok(booking);
        }

        [HttpPost("hold-seat")]
        public async Task<IActionResult> HoldSeat([FromBody] HoldSeatRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x => x.TripId == request.TripId && x.SeatId == request.SeatId);

                if (seat == null) return NotFound("Không tìm thấy ghế");

                if (seat.Status != "AVAILABLE")
                    return BadRequest("Ghế không khả dụng");

                seat.Status = "HELD";

                var hold = new SeatHold
                {
                    TripId = request.TripId,
                    SeatId = request.SeatId,
                    Status = "ACTIVE",
                    ExpiredAt = DateTime.Now.AddMinutes(10)
                };

                _context.SeatHolds.Add(hold);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok("Giữ ghế thành công");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookSeat([FromBody] BookSeatRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hold = await _context.SeatHolds
                    .FirstOrDefaultAsync(x =>
                        x.TripId == request.TripId &&
                        x.SeatId == request.SeatId &&
                        x.Status == "ACTIVE" &&
                        x.ExpiredAt > DateTime.Now);

                if (hold == null)
                    return BadRequest("Ghế chưa được giữ hoặc đã hết hạn");

                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null)
                    return NotFound("Customer không tồn tại");

                var booking = new Booking
                {
                    BookingCode = Guid.NewGuid().ToString(),
                    CustomerId = request.CustomerId,
                    TripId = request.TripId,
                    BookingDate = DateTime.Now,
                    Status = "CONFIRMED",
                    TotalAmount = 100
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var ticket = new Ticket
                {
                    BookingId = booking.Id,
                    TripId = request.TripId,
                    SeatId = request.SeatId,
                    Price = 100,
                    Status = "ISSUED"
                };

                _context.Tickets.Add(ticket);

                var seat = await _context.TripSeatInventories
                    .FirstAsync(x => x.TripId == request.TripId && x.SeatId == request.SeatId);

                seat.Status = "SOLD";
                hold.Status = "USED";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Đặt vé thành công");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.Status = "CANCELLED";

            var tickets = await _context.Tickets
                .Where(t => t.BookingId == id)
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x => x.TripId == ticket.TripId && x.SeatId == ticket.SeatId);

                if (seat != null)
                    seat.Status = "AVAILABLE";
            }

            await _context.SaveChangesAsync();
            return Ok("Đã hủy booking");
        }
    }
}