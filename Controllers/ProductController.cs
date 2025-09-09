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
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        private readonly IMapper _mapper;


        public ProductController(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Crea uno o muchos productos
        /// </summary>
        /// <param name="newProducts"></param>
        /// <param name="validador"></param>
        /// <returns></returns>
        [HttpPost("bulk-load")]
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

         
        await _context.Products.AddRangeAsync(_mapper.Map<List<ProductModel>>(newProducts));
            await _context.SaveChangesAsync();
            return Ok(ApiResult<object>.Ok(newProducts, "Producto creado en el catálogo exitosamente."));
        }

        /// <summary>
        /// Consulta toda la lista de productos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductCatalog()
        {
            var catalog = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            return Ok(ApiResult<object>.Ok(catalog, "Catálogo de productos recuperado exitosamente."));

        }
    }
}
