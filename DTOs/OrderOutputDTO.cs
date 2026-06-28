namespace BlazorAppEcommerce.DTOs
{
    public class OrderOutputDTO
    {
       
            public int order_Id { get; set; }
            public DateTime? orderDate { get; set; }
            public decimal totalAmount { get; set; }
        }
    }

