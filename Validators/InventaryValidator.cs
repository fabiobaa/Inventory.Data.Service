using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class InventaryValidator : AbstractValidator<List<DTOs.Inventory>>
    {
        private readonly InventoryDbContext _context;

        public InventaryValidator(InventoryDbContext context)
        {
            _context = context;

            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de inventario no puede estar vacía.")
                .CustomAsync(async (list, validationContext, cancellationToken) =>
                {
                    // 1. Obtener todos los IDs válidos de productos y tiendas en UNA sola consulta por tabla.
                    var validProductIds = await _context.Products
                        .AsNoTracking()
                        .Select(p => p.ProductId)
                        .ToHashSetAsync(cancellationToken);

                    var validStoreIds = await _context.Stores
                        .AsNoTracking()
                        .Select(s => s.StoreId)
                        .ToHashSetAsync(cancellationToken);

                    // 2. Detectar duplicados en memoria (StoreId + ProductId combinados).
                    var duplicados = list
                        .GroupBy(i => new { i.StoreId, i.ProductId })
                        .Where(g => g.Count() > 1)
                        .Select(g => $"StoreId={g.Key.StoreId}, ProductId={g.Key.ProductId}")
                        .ToList();

                    if (duplicados.Count != 0)
                    {
                        validationContext.AddFailure(
                            $"Existen registros duplicados en la lista: {string.Join(" | ", duplicados)}");
                    }

                    // 3. Validaciones individuales por item en memoria.
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
