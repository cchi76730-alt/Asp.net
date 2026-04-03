using Demo01.Data;
using Demo01.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreatePayment(int bookingId, decimal amount)
        {
            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = amount,
                Status = "SUCCESS"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }
    }
}