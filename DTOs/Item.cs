namespace BlazorAppEcommerce.DTOs
{
    /// <summary>
    /// Represents one product line inside a PlaceOrderDTO.
    /// </summary>
    public class Item
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}