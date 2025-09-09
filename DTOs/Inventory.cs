using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.DTOs
{
    public class Inventory
    {
        public string ProductId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
