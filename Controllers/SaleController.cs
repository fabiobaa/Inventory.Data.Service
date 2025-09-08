using Inventory.Data.Service.Models;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Inventory.Data.Service.Controllers
{
    [ApiController]
    [Route("api/sales")]
    public class SaleController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public SaleController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RecordSale([FromBody] SaleRequestDto saleRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var saleEvent = new SaleOccurredEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                TransactionId = saleRequest.TransactionId,
                ProductId = saleRequest.ProductId,
                StoreId = saleRequest.StoreId,
                QuantitySold = saleRequest.QuantitySold
            };

            var message = new QueuedMessage
            {
                Id = Guid.NewGuid(),
                Payload = JsonSerializer.Serialize(saleEvent),
                Status = "Pendiente",
                CreatedAt = DateTime.UtcNow
            };

            await _context.QueuedMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Accepted(new { messageId = message.Id, eventId = saleEvent.EventId });
        }
    }
}
