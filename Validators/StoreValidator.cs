using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class CreateStoresRequestValidator : AbstractValidator<List<Store>>
    {
        public CreateStoresRequestValidator(InventoryDbContext context)
        {
            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de tiendas no puede estar vacía.");

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
