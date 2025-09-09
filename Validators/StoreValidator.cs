using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class CreateStoresRequestValidator : AbstractValidator<List<Store>>
    {
        public CreateStoresRequestValidator(InventoryDbContext context)
        {
            // Regla 1: La lista en sí no puede estar vacía.
            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de tiendas no puede estar vacía.");

            // Regla 2: Detectar y mostrar duplicados de StoreId dentro de la lista.
            RuleFor(list => list)
                .Custom((list, contextValidation) =>
                {
                    var duplicados = list
                        .GroupBy(s => s.StoreId)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicados.Any())
                    {
                        contextValidation.AddFailure(
                            $"Las siguientes StoreId están duplicadas en la lista: {string.Join(", ", duplicados)}"
                        );
                    }
                });
            // Regla 3: Aplica el validador de tienda individual a CADA elemento de la lista.

            RuleForEach(list => list)
                .SetValidator(new StoreValidator(context));
        }
    }

    public class StoreValidator : AbstractValidator<Store>
    {
        private readonly InventoryDbContext _context;

        public StoreValidator(InventoryDbContext context)
        {
            _context = context;

            RuleFor(s => s.StoreId)
                .NotEmpty().WithMessage("El StoreId de la tienda es obligatorio.")
                .MaximumLength(20).WithMessage("El ID de la tienda no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueStoreId).WithMessage("La tienda con ID '{PropertyValue}' ya existe.");

            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("El nombre de la tienda es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }

        private async Task<bool> BeUniqueStoreId(string storeId, CancellationToken cancellationToken)
        {
            // La regla es válida si NO existe ninguna tienda con ese ID.
            return !await _context.Stores.AnyAsync(s => s.StoreId == storeId, cancellationToken);
        }
    }
}
