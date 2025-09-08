using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.DTOs
{
    public class SaleRequestDto
    {
        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string StoreId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int QuantitySold { get; set; }
    }
}
