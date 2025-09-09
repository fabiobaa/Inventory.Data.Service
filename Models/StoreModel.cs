using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.Models
{
    public class StoreModel
    {
        [Key]
        public string StoreId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
