using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.DTOs
{
    public class Sale
    {
        public string TransactionId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
    }
}
