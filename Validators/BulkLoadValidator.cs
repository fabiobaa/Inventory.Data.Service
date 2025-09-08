using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class BulkLoadValidator : AbstractValidator<List<InventoryProducts>>
    {
        private readonly InventoryDbContext _context;
        public BulkLoadValidator(InventoryDbContext context)
        {

            _context = context;
            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de productos no puede estar vacía.")
                .CustomAsync(async (list, validationContext, cancellationToken) =>
                {
                    // 1. Hacemos UNA SOLA consulta para obtener todos los IDs de productos válidos.
                    var validProductIds = await _context.Products
                        .Select(p => p.ProductId)
                        .ToHashSetAsync(cancellationToken);

                    // 2. Hacemos UNA SOLA consulta para obtener todos los IDs de tiendas válidas.
                    var validStoreIds = await _context.Stores
                        .Select(s => s.StoreId)
                        .ToHashSetAsync(cancellationToken);

                    // 3. Validamos cada item de la lista en memoria (muy rápido).
                    foreach (var item in list)
                    {
                        if (!validProductIds.Contains(item.ProductId))
                        {
                            validationContext.AddFailure(
                                nameof(item.ProductId),
                                $"El producto con ID '{item.ProductId}' no existe en el catálogo.");
                        }

                        if (!validStoreIds.Contains(item.StoreId))
                        {
                            validationContext.AddFailure(
                                nameof(item.StoreId),
                                $"La tienda con ID '{item.StoreId}' no existe.");
                        }

                        if (item.Quantity < 0)
                        {
                            validationContext.AddFailure(
                               nameof(item.Quantity),
                               $"La cantidad para el producto '{item.ProductId}' no puede ser negativa.");
                        }
                    }
                });
        }
    }
}
