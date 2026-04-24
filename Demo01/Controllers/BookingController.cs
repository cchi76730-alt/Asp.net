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

        private const string AVAILABLE = "AVAILABLE";
        private const string HELD = "HELD";
        private const string SOLD = "SOLD";

        // ================= CLEAR EXPIRED =================
        private async Task ClearExpiredHolds()
        {
            var expired = await _context.SeatHolds
.Where(x => x.ExpiredAt < DateTime.Now).ToListAsync();

            foreach (var h in expired)
            {
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == h.TripId &&
                        x.SeatId == h.SeatId);

                if (seat != null && (seat.Status ?? "") == HELD)
                    seat.Status = AVAILABLE;
            }

            _context.SeatHolds.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }

        // ================= HOLD SEAT =================
        [HttpPost("hold-seat")]
        public async Task<IActionResult> HoldSeat([FromBody] HoldSeatRequest request)
        {
            try
            {
                //await ClearExpiredHolds();

                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == request.TripId &&
                        x.SeatId == request.SeatId);

                if (seat == null)
                    return NotFound("Không tìm thấy ghế");

                var status = (seat.Status ?? AVAILABLE).ToUpper();

                if (status == SOLD)
                    return BadRequest("Ghế đã bán");

                if (status == HELD)
                    return BadRequest("Ghế đang được giữ");

                seat.Status = HELD;

                _context.SeatHolds.Add(new SeatHold
                {
                    TripId = request.TripId,
                    SeatId = request.SeatId,
                    CustomerId = 1,
                    Status = "ACTIVE",
                    ExpiredAt = DateTime.Now.AddMinutes(10)
                });

                await _context.SaveChangesAsync();

                return Ok("HOLD OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR BOOK:");
                Console.WriteLine(ex.ToString());

                // 👇 THÊM DÒNG NÀY để thấy lỗi thật
                if (ex.InnerException != null)
                    Console.WriteLine("INNER: " + ex.InnerException.Message);

                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        // ================= BOOK =================
        [HttpPost("book")]
        public async Task<IActionResult> CreateBooking([FromBody] BookSeatRequest request)
        {
            try
            {
                var hold = await _context.SeatHolds
                    .FirstOrDefaultAsync(x =>
                        x.TripId == request.TripId &&
                        x.SeatId == request.SeatId &&
                        x.Status == "ACTIVE");

                if (hold == null)
                    return BadRequest("Ghế chưa được giữ hoặc đã hết hạn");

                var booking = new Booking
                {
                    BookingCode = Guid.NewGuid().ToString(),
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName ?? "",
                    Phone = request.Phone ?? "",
                    TripId = request.TripId,
                    SeatId = request.SeatId,
                    BookingDate = DateTime.Now,
                    Status = "PENDING_PAYMENT",
                    TotalAmount = 100,
                    IsPaid = false
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR BOOK:");
                Console.WriteLine(ex.ToString());

                // 👇 rollback ghế nếu lỗi
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == request.TripId &&
                        x.SeatId == request.SeatId);

                if (seat != null)
                    seat.Status = AVAILABLE;

                await _context.SaveChangesAsync();

                return StatusCode(500, ex.Message);
            }
        }

        // ================= SEATS =================
        [HttpGet("seats/{tripId}")]
        public async Task<IActionResult> GetSeats(int tripId)
        {
            try
            {
                var seats = await _context.TripSeatInventories
                    .Where(x => x.TripId == tripId)
                    .Select(x => new
                    {
                        x.Id,
                        x.SeatId,
                        Status = x.Status ?? AVAILABLE
                    })
                    .ToListAsync();

                return Ok(seats);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR GET SEATS:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // ================= PAY =================
        [HttpPost("pay/{bookingId}")]
        public async Task<IActionResult> Pay(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                if (booking == null)
                    return NotFound("Không tìm thấy booking");

                if (booking.Status != "PENDING_PAYMENT")
                    return BadRequest("Booking không hợp lệ");
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == booking.TripId &&
                        x.SeatId == booking.SeatId);

                if (seat == null)
                    return BadRequest("Không tìm thấy ghế");

                booking.Status = "CONFIRMED";
                booking.IsPaid = true;

                _context.Tickets.Add(new Ticket
                {
                    BookingId = booking.Id,
                    TripId = booking.TripId,
                    SeatId = booking.SeatId,
                    Price = 100,
                    Status = "ISSUED"
                });

                seat.Status = SOLD;

                await _context.SaveChangesAsync();

                return Ok("Thanh toán thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR PAY:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // ================= FAIL =================
        [HttpPost("fail/{bookingId}")]
        public async Task<IActionResult> FailPayment(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);

                if (booking == null)
                    return NotFound("Không tìm thấy booking");

                booking.Status = "FAILED";

                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == booking.TripId &&
                        x.SeatId == booking.SeatId);

                if (seat != null)
                    seat.Status = AVAILABLE;

                await _context.SaveChangesAsync();

                return Ok("Payment failed - seat released");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR BOOK:");
                Console.WriteLine(ex.ToString());

                if (ex.InnerException != null)
                    Console.WriteLine("INNER: " + ex.InnerException.Message);

                // ❌ KHÔNG gọi SaveChanges nữa
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        // ================= DTO =================
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
            public string CustomerName { get; set; } = "";
            public string Phone { get; set; } = "";
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetTicketsByCustomer(int customerId)
        {
            try
            {
                var data = await _context.Bookings
.Where(b => b.CustomerId == customerId).Select(b => new
                    {
                        b.Id,
                        b.BookingCode,
                        b.CustomerName,
                        b.Phone,
                        b.TripId,
                        b.SeatId, // ✅ sửa lại
                        b.BookingDate,
                        b.Status,
                        b.IsPaid
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR GET CUSTOMER TICKETS:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                    return NotFound("Không tìm thấy vé");

                // 👇 tìm ghế
                var seat = await _context.TripSeatInventories
                    .FirstOrDefaultAsync(x =>
                        x.TripId == booking.TripId &&
                        x.SeatId == booking.SeatId);

                if (seat != null)
                    seat.Status = AVAILABLE; // trả ghế lại

                // 👇 xóa ticket nếu có
                var tickets = _context.Tickets
                    .Where(t => t.BookingId == booking.Id);

                _context.Tickets.RemoveRange(tickets);

                // 👇 xóa booking
                _context.Bookings.Remove(booking);

                await _context.SaveChangesAsync();

                return Ok("Xóa vé thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR DELETE:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookSeatRequest request)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                    return NotFound("Không tìm thấy vé");

                if (booking.IsPaid)
                    return BadRequest("Vé đã thanh toán, không thể sửa");

                // ✅ chỉ sửa thông tin
                booking.CustomerName = request.CustomerName;
                booking.Phone = request.Phone;

                await _context.SaveChangesAsync();

                return Ok("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR UPDATE:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }
    }


}
