using static MudBlazor.CategoryTypes;

namespace BlazorAppEcommerce.DTOs
{
    public class PlaceOrderDTO
    {
        public int UserId { get; set; }
        public List<Item> Items { get; set; }
    }
}
