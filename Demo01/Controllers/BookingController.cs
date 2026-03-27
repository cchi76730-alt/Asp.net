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

        // Đặt ghế
        [HttpPost("book")]
        public async Task<IActionResult> BookSeat(int tripId, int seatId, int customerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra ghế còn không
                var seatInventory = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x => x.TripId == tripId && x.SeatId == seatId);

                if (seatInventory == null)
                    return NotFound("Không tìm thấy ghế");

                if (seatInventory.IsBooked)
                    return BadRequest("Ghế đã được đặt");

                // Đánh dấu ghế đã đặt
                seatInventory.IsBooked = true;

                // Tạo booking
                var booking = new Booking
                {
                    CustomerId = customerId,
                    BookingDate = DateTime.Now
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Tạo ticket
                var ticket = new Ticket
                {
                    BookingId = booking.Id,
                    TripId = tripId,
                    SeatId = seatId,
                    Price = 100
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Đặt vé thành công",
                    BookingId = booking.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Đặt vé thất bại");
            }
        }

        // Xem danh sách booking
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return Ok(bookings);
        }
    }
}