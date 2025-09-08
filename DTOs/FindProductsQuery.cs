namespace Inventory.Data.Service.DTOs
{
    public class FindProductsQuery
    {
        public string? StoreId { get; set; }
        public string? ProductId { get; set; }
    }
}
