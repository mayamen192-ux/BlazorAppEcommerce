namespace BlazorAppEcommerce.DTOs
{
    /// <summary>
    /// Sent to OrderService.AddOrder — contains the buyer's ID
    /// and every product line they are ordering.
    /// </summary>
    public class PlaceOrderDTO
    {
        public int UserId { get; set; }
        public List<Item> Items { get; set; } = new();
    }

   
    
}