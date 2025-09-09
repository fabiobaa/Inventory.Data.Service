using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Inventory.Data.Service.Shared;
using Inventory.Data.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Controllers
{
    [ApiController]
    [Route("api/queue")]
    public class QueueController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public QueueController(InventoryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene los mensajes de la cola, permitiendo filtrar por estado y paginación
        /// </summary>
        /// <param name="query">Parámetros de consulta (estado, límite, página)</param>
        /// <param name="validator">Validador para los parámetros de consulta</param>
        /// <returns>Lista de mensajes de la cola</returns>
        [HttpGet("messages")]
        public async Task<IActionResult> GetQueueMessages(
            [FromServices] QueueQueryValidator validator , 
            [FromQuery] QueueQuery query)
        {

            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errors, "Parámetros de consulta inválidos"));
            }

            var dbQuery = _context.QueuedMessages
                .OrderByDescending(m => m.CreatedAt)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                dbQuery = dbQuery.Where(m => m.Status.Equals(query.Status, StringComparison.OrdinalIgnoreCase));
            }

            // Aplicar paginación si se especifica
            if (query.Page.HasValue)
            {
                var skip = (query.Page.Value - 1) * query.Limit;
                dbQuery = dbQuery.Skip(skip);
            }

            var messages = await dbQuery.Take(query.Limit).ToListAsync();

            var response = new
            {
                messages,
                totalCount = await _context.QueuedMessages.CountAsync(),
                page = query.Page ?? 1,
                limit = query.Limit,
                hasMore = messages.Count == query.Limit
            };

            return Ok(ApiResult<object>.Ok(response, "Mensajes de la cola recuperados exitosamente"));
        }
    }
}
