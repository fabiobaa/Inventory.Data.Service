using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory.Data.Service.Models
{
    /// <summary>
    /// Representa un ítem de inventario en una tienda específica.
    /// </summary>
    public class InventoryModel
    {
        [Key]
        public string ProductId { get; set; } = string.Empty;
        [Key]
        public string StoreId { get; set; } = string.Empty;
        public int Quantity { get; set; }

        /// <summary>
        /// Campo para el control de concurrencia optimista.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[]? Version { get; set; }
    }
}
