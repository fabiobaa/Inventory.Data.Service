using FluentValidation;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;
using Inventory.Data.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Validators
{
    public class CreateProductsRequestValidator : AbstractValidator<List<Product>>
    {
        public CreateProductsRequestValidator(InventoryDbContext context)
        {
            // Regla 1: La lista en sí no puede estar vacía.
            RuleFor(list => list)
                .NotEmpty().WithMessage("La lista de productos no puede estar vacía.");

            // Regla 2: No permitir ProductId duplicados dentro de la lista.
            RuleFor(list => list)
           .Custom((list, contextValidation) =>
           {
               var duplicados = list
                   .GroupBy(p => p.ProductId)
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key)
                   .ToList();

               if (duplicados.Any())
               {
                   contextValidation.AddFailure(
                       $"Los siguientes ProductId están duplicados en la lista: {string.Join(", ", duplicados)}"
                   );
               }
           });

            // Regla 3: Aplica el validador de producto individual a CADA elemento de la lista.
            RuleForEach(list => list)
                .SetValidator(new ProductValidator(context));
        }
    }

    /// <summary>
    /// Validador anidado que contiene las reglas para UN SOLO producto.
    /// </summary>
    public class ProductValidator : AbstractValidator<Product>
    {
        private readonly InventoryDbContext _context;

        public ProductValidator(InventoryDbContext context)
        {
            _context = context;

            RuleFor(p => p.ProductId)
                .NotEmpty().WithMessage("El ID del producto es obligatorio.")
                .MaximumLength(50).WithMessage("El ID del producto no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueProductId).WithMessage("El producto con ID '{PropertyValue}' ya existe en el catálogo.");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }

        private async Task<bool> BeUniqueProductId(string productId, CancellationToken cancellationToken)
        {
            // La regla es válida si NO existe ningún producto con ese ID.
            return !await _context.Products.AnyAsync(p => p.ProductId == productId, cancellationToken);
        }
    }
}
