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

        // =========================
        // GET ALL BOOKINGS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tickets)
                .ToListAsync();

            return Ok(bookings);
        }

        // =========================
        // GET BOOKING BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // =========================
        // HOLD SEAT
        // =========================
        [HttpPost("hold-seat")]
        public async Task<IActionResult> HoldSeat(int tripId, int seatId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var seat = await _context.TripSeatInventories
                .FirstOrDefaultAsync(x => x.TripId == tripId && x.SeatId == seatId);

            if (seat == null)
                return NotFound("Không tìm thấy ghế");

            if (seat.Status != "AVAILABLE")
                return BadRequest("Ghế không khả dụng");

            seat.Status = "HELD";

            var hold = new SeatHold
            {
                TripId = tripId,
                SeatId = seatId,
                Status = "ACTIVE",
                ExpiredAt = DateTime.Now.AddMinutes(10)
            };

            _context.SeatHolds.Add(hold);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok("Giữ ghế thành công");
        }

        // =========================
        // BOOK SEAT
        // =========================
        [HttpPost("book")]
        public async Task<IActionResult> BookSeat(int tripId, int seatId, int customerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var seat = await _context.TripSeatInventories
                .FirstOrDefaultAsync(x => x.TripId == tripId && x.SeatId == seatId);

            if (seat == null)
                return NotFound("Không tìm thấy ghế");

            if (seat.Status != "HELD")
                return BadRequest("Ghế chưa được giữ");

            var booking = new Booking
            {
                BookingCode = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                TripId = tripId,
                BookingDate = DateTime.Now,
                Status = "CONFIRMED",
                TotalAmount = 100
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                BookingId = booking.Id,
                TripId = tripId,
                SeatId = seatId,
                Price = 100,
                Status = "ISSUED"
            };

            _context.Tickets.Add(ticket);

            seat.Status = "SOLD";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok("Đặt vé thành công");
        }

        // =========================
        // CANCEL BOOKING
        // =========================
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
                return NotFound();

            booking.Status = "CANCELLED";

            var tickets = await _context.Tickets
                .Where(t => t.BookingId == bookingId)
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

        // =========================
        // RELEASE EXPIRED
        // =========================
        [HttpPost("release-expired")]
        public async Task<IActionResult> ReleaseExpiredSeats()
        {
            var now = DateTime.Now;

            var expiredHolds = await _context.SeatHolds
                .Where(x => x.Status == "ACTIVE" && x.ExpiredAt < now)
                .ToListAsync();

            foreach (var hold in expiredHolds)
            {
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x => x.TripId == hold.TripId && x.SeatId == hold.SeatId);

                if (seat != null)
                    seat.Status = "AVAILABLE";

                hold.Status = "EXPIRED";
            }

            await _context.SaveChangesAsync();

            return Ok("Đã release ghế hết hạn");
        }
    }
}