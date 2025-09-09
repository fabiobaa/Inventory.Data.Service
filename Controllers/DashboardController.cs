using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.Shared;

namespace Inventory.Data.Service.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public DashboardController(InventoryDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Consulta las cantidades 
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        public async Task<IActionResult> GetGlobalSummary()
        {
            // Realizamos todas las consultas de agregación en paralelo para máxima eficiencia
            var productCountTask = _context.Products.CountAsync();
            var storeCountTask = _context.Stores.CountAsync();
            var inventoryRecordsTask = _context.Inventory.CountAsync();
            var totalUnitsTask = _context.Inventory.SumAsync(i => (long)i.Quantity); // Usamos long para evitar desbordamiento

            var queueSummaryTask = _context.QueuedMessages
                .GroupBy(m => m.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            // Esperamos a que todas las tareas terminen
            await Task.WhenAll(
                productCountTask,
                storeCountTask,
                inventoryRecordsTask,
                totalUnitsTask,
                queueSummaryTask);

            // Construimos el objeto de respuesta
            var summary = new
            {
                TotalProductsInCatalog = productCountTask.Result,
                TotalStores = storeCountTask.Result,
                TotalInventoryRecords = inventoryRecordsTask.Result,
                TotalUnitsInStock = totalUnitsTask.Result,
                QueueStatus = queueSummaryTask.Result
            };

            return Ok(ApiResult<object>.Ok(summary, "Resumen global del sistema recuperado exitosamente."));

        }
    }
}