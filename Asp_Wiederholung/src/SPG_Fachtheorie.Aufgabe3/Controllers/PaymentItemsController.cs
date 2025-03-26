using Microsoft.AspNetCore.Mvc;
using SPG_Fachtheorie.Aufgabe3.Controllers.Commands;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentItemsController : ControllerBase
    {
        private readonly AppointmentContext _context;

        public PaymentItemsController(AppointmentContext context)
        {
            _context = context;
        }

        // PUT: api/paymentItems/{id}
        [HttpPut("{id}")]
        public IActionResult PutPaymentItem(int id, [FromBody] UpdatePaymentItemCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Invalid payment item ID" });

            var existingItem = _context.PaymentItems.Find(id);
            if (existingItem == null)
                return NotFound(new { message = "Payment Item not found" });

            if (existingItem.LastUpdated != command.LastUpdated)
                return BadRequest(new { message = "Payment item has changed" });

            var paymentExists = _context.Payments.Any(p => p.Id == command.PaymentId);
            if (!paymentExists)
                return BadRequest(new { message = "Invalid payment ID" });

            existingItem.ArticleName = command.ArticleName;
            existingItem.Amount = command.Amount;
            existingItem.Price = command.Price;
            existingItem.PaymentId = command.PaymentId;
            existingItem.LastUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return NoContent();
        }
    }
}
