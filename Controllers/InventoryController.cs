using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Inventory.Data.Service.Models;
using Inventory.Data.Service.Shared;
using Inventory.Data.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : Controller
    {
        private readonly InventoryDbContext _context;

        public InventoryController(InventoryDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> FindInventoryProducts(
            [FromServices] FindProductsQueryValidator validador,
            [FromQuery] string? storeId = null, [FromQuery] string? productId = null)
        {

            // 1. Validación FluentValidation
            var validationResult = validador.Validate(new FindProductsQuery
            {
                ProductId = productId,
                StoreId = storeId
            });
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errores, "Validación de datos fallida"));
            }


            var query = _context.InventoryProducts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(storeId))
            {
                query = query.Where(item => item.StoreId.ToLower() == storeId.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(productId))
            {
                query = query.Where(item => item.ProductId.ToLower() == productId.ToLower());
            }

            var result = await query.ToListAsync();
            return Ok(ApiResult<object>.Ok(
               new
               {
                   Success = true,
                   Data = result,
                   Message = "Productos recuperados exitosamente"
               }
           ));
        }

        [HttpPost("bulk-load")]

        public async Task<IActionResult> BulkLoadInventory(
            [FromBody] List<InventoryProducts> inventory,
            [FromServices] BulkLoadValidator validador
            )
        {

            var validationResult = await validador.ValidateAsync(inventory);
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errores, "Validación de datos fallida"));
            }

            var existingProducts = await _context.InventoryProducts
                .Where(dbProduct => inventory.Select(product => product.ProductId).Contains(dbProduct.ProductId) &&
                                 inventory.Select(item => item.StoreId).Contains(dbProduct.StoreId))
                .ToDictionaryAsync(i => $"{i.StoreId}-{i.ProductId}");

            foreach (var item in inventory)
            {
                var key = $"{item.StoreId}-{item.ProductId}";

                if (existingProducts.TryGetValue(key, out var trackedItem))
                {
                    trackedItem.Quantity = item.Quantity;
                }
                else
                {
                    _context.InventoryProducts.Add(item);
                    existingProducts.Add(key, item);
                }
            }

            await _context.SaveChangesAsync();
            var responseData = new { itemsProcessed = inventory.Count };
            return Ok(ApiResult<object>.Ok(inventory, "Inventario cargado exitosamente."));
        }
    }
}
