using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.Models
{
    public class ProductModel
    {
        [Key]
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

    }
}
