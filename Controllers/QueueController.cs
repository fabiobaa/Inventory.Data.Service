using Inventory.Data.Service.Data;
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
        /// Obtiene los mensajes de la cola, permitiendo filtrar por estado.
        /// </summary>
        /// <param name="status">Filtro opcional por estado (ej. "Pendiente", "Procesado", "Error").</param>
        /// <param name="limit">Limita el número de resultados (por defecto 50).</param>
        [HttpGet("messages")]
        public async Task<IActionResult> GetQueueMessages([FromQuery] string? status = null, [FromQuery] int limit = 50)
        {
            var query = _context.QueuedMessages
                                .OrderByDescending(m => m.CreatedAt) // Muestra los más recientes primero
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.Status.Equals(status, StringComparison.CurrentCultureIgnoreCase));
            }

            // Aplicamos el límite para no devolver demasiados datos
            var messages = await query.Take(limit).ToListAsync();

            return Ok(messages);
        }
    }
}
