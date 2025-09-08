using Inventory.Data.Service.Data;
using Inventory.Data.Service.Models;
using Inventory.Data.Service.Shared;
using Inventory.Data.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Controllers
{

    [ApiController]
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public StoreController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpPost("bulk-load")]
        public async Task<IActionResult> CreateStore(
            [FromBody] List<Store> newStores, 
            [FromServices] CreateStoresRequestValidator validador)
        {
            var validationResult = await validador.ValidateAsync(newStores);
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errores, "Validación de datos fallida"));
            }

            _context.Stores.AddRange(newStores);
            await _context.SaveChangesAsync();

            return Ok(ApiResult<object>.Ok(newStores, "Tienda creada exitosamente."));
        }

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.Stores.OrderBy(s => s.Name).ToListAsync();
            return Ok(ApiResult<object>.Ok(stores, "Tiendas recuperadas exitosamente."));
        }
    }
}
