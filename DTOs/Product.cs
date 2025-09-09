using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.DTOs
{
    public class Product
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
