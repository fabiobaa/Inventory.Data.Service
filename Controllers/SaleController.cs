using Inventory.Data.Service.Models;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Inventory.Data.Service.Validators;
using Inventory.Data.Service.Shared;
using AutoMapper;

namespace Inventory.Data.Service.Controllers
{
    [ApiController]
    [Route("api/sales")]
    public class SaleController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        private readonly IMapper _mapper;

        public SaleController(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // <summary>
        /// Registra una nueva venta en el sistema de forma asíncrona
        /// </summary>
        /// <param name="sale">Datos de la venta a registrar</param>
        /// <param name="validator">Validador para la solicitud de venta</param>
        /// <returns>ID del mensaje en cola y del evento generado</returns>
        [HttpPost("bulk-load")]
        public async Task<IActionResult> RecordSale([FromBody] List<Sale> sales, [FromServices] SaleValidator validator)
        {
            var validationResult = await validator.ValidateAsync(sales);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(ApiResult<object>.Fail(errors, "Validación de datos fallida"));
            }

            var salesevents = _mapper.Map<List<SaleOccurredEvent>>(sales);
            var queues = _mapper.Map<List<QueuedMessageModel>>(salesevents);

            await _context.QueuedMessages.AddRangeAsync(queues);
            await _context.SaveChangesAsync();

            return Accepted();
        }
    }
}
