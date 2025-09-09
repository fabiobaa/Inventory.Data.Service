namespace Inventory.Data.Service.DTOs
{
    public class QueueQuery
    {
        public int Limit { get; set; } = 1;
        public string? Status { get; set; }
        public int? Page { get; set; }
    }
}
