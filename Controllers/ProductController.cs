using Inventory.Data.Service.Data;
using Inventory.Data.Service.Models;
using Inventory.Data.Service.Shared;
using Inventory.Data.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public ProductController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpPost("catalog")]
        public async Task<IActionResult> CreateProductInCatalog(
            [FromBody] List<Product> newProducts,
            [FromServices] CreateProductsRequestValidator validador)
        {
            var validationResult = await validador.ValidateAsync(newProducts);
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errores, "Validación de datos fallida"));
            }

            await _context.Products.AddRangeAsync(newProducts);
            await _context.SaveChangesAsync();
            return Ok(ApiResult<object>.Ok(newProducts, "Producto creado en el catálogo exitosamente."));
        }

        [HttpGet("catalog")]
        public async Task<IActionResult> GetProductCatalog()
        {
            var catalog = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            return Ok(ApiResult<object>.Ok(catalog, "Catálogo de productos recuperado exitosamente."));

        }
    }
}
