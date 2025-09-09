using FluentValidation;
using Inventory.Data.Service.DTOs;
using Inventory.Data.Service.Models;

namespace Inventory.Data.Service.Validators
{
    public class QueueQueryValidator : AbstractValidator<QueueQuery>
    {
        public QueueQueryValidator()
        {
            RuleFor(x => x.Status)
                .Must(BeValidStatus).WithMessage("El estado debe ser uno de: 'Pendiente', 'Procesado', 'Error'")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 1000).WithMessage("El límite debe estar entre 1 y 1000");

            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("El número de página debe ser mayor a 0")
                .When(x => x.Page.HasValue);
        }

        private bool BeValidStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return true;

            var validStatuses = new[] { MessageStatus.Pendiente, MessageStatus.Procesado, MessageStatus.Error };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}
