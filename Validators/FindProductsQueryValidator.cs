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
            }
        }
}
