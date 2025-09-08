using FluentValidation;
using Inventory.Data.Service.DTOs;

namespace Inventory.Data.Service.Validators
{
        public class FindProductsQueryValidator : AbstractValidator<FindProductsQuery>
        {
            public FindProductsQueryValidator()
            {
                // Regla para StoreId: si se proporciona, no puede estar vacío y debe tener un máximo de 20 caracteres.
                RuleFor(x => x.StoreId)
                    .MaximumLength(20).WithMessage("StoreId no puede tener más de 20 caracteres.")
                    .When(x => !string.IsNullOrWhiteSpace(x.StoreId));

                // Regla para ProductId: si se proporciona, no puede estar vacío y debe seguir un formato (ej. ABC-123).
                RuleFor(x => x.ProductId)                   
                    .Matches(@"^[A-Z]+-[0-9]+$").WithMessage("El formato de ProductId debe ser 'CATEGORIA-NUMERO', por ejemplo, 'BOOKS-123'.")
                    .When(x => !string.IsNullOrWhiteSpace(x.ProductId));
            }
        }
}
