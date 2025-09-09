using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class SaleValidator : AbstractValidator<List<Sale>>
    {
        private readonly InventoryDbContext _context;
        public SaleValidator(InventoryDbContext context)
        {
            _context = context;
            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de ventas no puede estar vacía.")
                // Validador personalizado y optimizado para toda la lista.
                .CustomAsync(async (sales, validationContext, cancellationToken) =>
                {
                    // 1. Obtener todos los IDs únicos de la petición en una sola pasada.
                    var productIds = sales.Select(s => s.ProductId).Distinct().ToList();
                    var storeIds = sales.Select(s => s.StoreId).Distinct().ToList();

                    // 2. Hacer consultas eficientes a la base de datos una sola vez.
                    var existingProductIds = await _context.Products
                    .AsNoTracking()
                    .Where(p => productIds.Contains(p.ProductId))
                    .Select(p => p.ProductId)
                    .ToHashSetAsync(cancellationToken);

                    var existingStoreIds = await _context.Stores
                        .AsNoTracking()
                        .Where(s => storeIds.Contains(s.StoreId))
                        .Select(s => s.StoreId)
                        .ToHashSetAsync(cancellationToken);

                    var inventoryLevels = await _context.Inventory
                        .AsNoTracking()
                        .Where(i => productIds.Contains(i.ProductId) && storeIds.Contains(i.StoreId))
                        .Select(i => new { Key = i.StoreId + "-" + i.ProductId, i.Quantity })
                        .ToDictionaryAsync(x => x.Key, x => x.Quantity, cancellationToken);

                    // 3. Validar cada venta en memoria 
                    for (int i = 0; i < sales.Count; i++)
                    {
                        var sale = sales[i];

                        if (!existingProductIds.Contains(sale.ProductId))
                        {
                            validationContext.AddFailure($"sales[{i}].ProductId", $"El producto con ID '{sale.ProductId}' no existe en el catálogo.");
                        }

                        if (!existingStoreIds.Contains(sale.StoreId))
                        {
                            validationContext.AddFailure($"sales[{i}].StoreId", $"La tienda con ID '{sale.StoreId}' no existe.");
                        }

                        if (sale.QuantitySold <= 0)
                        {
                            validationContext.AddFailure($"sales[{i}].QuantitySold",
                                $"La cantidad vendida debe ser mayor que 0. Valor recibido: {sale.QuantitySold}.");
                            continue;
                        }

                        // Solo si el producto y la tienda existen, validamos el stock.
                        if (existingProductIds.Contains(sale.ProductId) && existingStoreIds.Contains(sale.StoreId))
                        {
                            var key = $"{sale.StoreId}-{sale.ProductId}";
                            inventoryLevels.TryGetValue(key, out var currentStock); // Si no hay registro, el stock es 0.

                            if (currentStock < sale.QuantitySold)
                            {
                                validationContext.AddFailure($"sales[{i}].QuantitySold",
                                    $"No hay suficiente stock para el producto '{sale.ProductId}'. Stock actual: {currentStock}, se requieren: {sale.QuantitySold}.");
                            }
                        }
                    }
                });
        }
    }
}
