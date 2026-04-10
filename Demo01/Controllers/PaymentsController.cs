using Demo01.Data;
using Demo01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        public class CreatePaymentRequest
        {
            public int BookingId { get; set; }
            public decimal Amount { get; set; }
        }

        // =========================
        // CREATE PAYMENT
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var booking = await _context.Bookings.FindAsync(request.BookingId);
            if (booking == null)
                return NotFound("Booking không tồn tại");

            if (request.Amount <= 0)
                return BadRequest("Số tiền không hợp lệ");

            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                Status = "PENDING"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // =========================
        // GET PAYMENT BY BOOKING
        // =========================
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetByBooking(int bookingId)
        {
            var payments = await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();

            return Ok(payments);
        }

        // =========================
        // CONFIRM PAYMENT
        // =========================
        [HttpPost("confirm/{paymentId}")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return NotFound();

            payment.Status = "SUCCESS";

            var booking = await _context.Bookings.FindAsync(payment.BookingId);
            if (booking != null)
                booking.Status = "PAID";

            await _context.SaveChangesAsync();

            return Ok("Thanh toán thành công");
        }
    }
}