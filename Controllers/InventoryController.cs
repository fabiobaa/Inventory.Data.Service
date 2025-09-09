using AutoMapper;
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
        private readonly IMapper _mapper;


        public InventoryController(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        /// <summary>
        /// Consulta inventario 
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FindInventoryProducts(
            [FromServices] FindProductsQueryValidator validator,
            [FromQuery] FindProductsQuery findProductsQuery)
        {

            var validationResult = await validator.ValidateAsync(findProductsQuery);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errors, "Validación de datos fallida"));
            }

            var query = _context.Inventory.AsQueryable();

            if (!string.IsNullOrWhiteSpace(findProductsQuery.StoreId))
            {
                query = query.Where(item => item.StoreId.ToLower() == findProductsQuery.StoreId.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(findProductsQuery.ProductId))
            {
                query = query.Where(item => item.ProductId.ToLower() == findProductsQuery.ProductId.ToLower());
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


        // <summary>
        /// Carga masiva de inventario
        /// </summary>
        /// <param name="inventory">Lista de productos de inventario a cargar</param>
        /// <param name="validator">Validador para la carga masiva</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("bulk-load")]
        public async Task<IActionResult> BulkLoadInventory(
            [FromBody] List<DTOs.Inventory> inventory,
            [FromServices] InventaryValidator validador
            )
        {

            var validationResult = await validador.ValidateAsync(inventory);
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errores, "Validación de datos fallida"));
            }

            var existingProducts = await _context.Inventory
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
                    var data = new InventoryModel()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId

                    };
                    _context.Inventory.Add(_mapper.Map<InventoryModel>(item));
                    existingProducts.Add(key, data);
                }
            }

            await _context.SaveChangesAsync();
            var responseData = new { itemsProcessed = inventory.Count };
            return Ok(ApiResult<object>.Ok(inventory, "Inventario cargado exitosamente."));
        }
    }
}
