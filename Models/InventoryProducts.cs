using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory.Data.Service.Models
{
    /// <summary>
    /// Representa un ítem de inventario en una tienda específica.
    /// </summary>
    public class InventoryProducts
    {

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string StoreId { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Campo para el control de concurrencia optimista.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[]? Version { get; set; }
    }
}
