using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.DTOs
{
    public class SaleOccurredEvent
    {
        // Un ID único para este evento específico.
        [Required]
        public Guid EventId { get; set; } = Guid.NewGuid();

        // El ID de la transacción real (ej. número de recibo de la tienda).
        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string StoreId { get; set; } = string.Empty;

        // La cantidad vendida debe ser al menos 1.
        [Range(1, int.MaxValue)]
        public int QuantitySold { get; set; }

        // La fecha y hora en que ocurrió la venta.
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
