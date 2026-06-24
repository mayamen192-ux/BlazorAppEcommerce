using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazorAppEcommerce.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================
        // ADD ORDER
        // =====================
        public async Task<int> AddOrder(Order order)
        {
            try
            {
                _logger.LogInformation(
                    "Placing new order | UserID: {UserId} | TotalAmount: {TotalAmount}",
                    order.UserId, order.TotalAmount);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Order placed successfully | OrderID: {OrderId} | UserID: {UserId}",
                    order.order_Id, order.UserId);

                return order.order_Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while placing order for UserID: {UserId}",
                    order.UserId);

                return 0;
            }
        }
    }
}