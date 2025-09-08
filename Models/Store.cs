using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.Models
{
    public class Store
    {
        [Key]
        public string StoreId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
