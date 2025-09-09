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
    [Route("api/stores")]
    public class StoreController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        private readonly IMapper _mapper;


        public StoreController(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Crear una o muchas tiendas
        /// </summary>
        /// <param name="newStores"></param>
        /// <param name="validador"></param>
        /// <returns></returns>
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

            _context.Stores.AddRange(_mapper.Map<List<StoreModel>>(newStores));
           
            await _context.SaveChangesAsync();

            return Ok(ApiResult<object>.Ok(newStores, "Tienda creada exitosamente."));
        }

        /// <summary>
        /// Consulta todas las tiendas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.Stores.OrderBy(s => s.Name).ToListAsync();
            return Ok(ApiResult<object>.Ok(stores, "Tiendas recuperadas exitosamente."));
        }
    }
}
