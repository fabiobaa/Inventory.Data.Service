using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Service.Models
{
    public class QueuedMessageModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Payload { get; set; } = string.Empty;
        public string Status { get; set; } = MessageStatus.Pendiente;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public static class MessageStatus
    {
        public const string Pendiente = "Pendiente";
        public const string Procesado = "Procesado";
        public const string Error = "Error";
    }
}
