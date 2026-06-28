using BlazorAppEcommerce.DTOs;
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

        // ─────────────────────────────────────────────
        // PLACE ORDER  (accepts DTO, returns OrderOutput)
        // ─────────────────────────────────────────────
        public async Task<OrderOutput?> AddOrder(PlaceOrderDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
            {
                _logger.LogWarning("AddOrder called with no items for UserID: {UserId}", dto.UserId);
                return null;
            }

            try
            {
                _logger.LogInformation(
                    "Placing order | UserID: {UserId} | Items: {Count}",
                    dto.UserId, dto.Items.Count);

                // Map each DTO Item → OrderProducts entity
                var orderProducts = dto.Items.Select(item => new OrderProducts
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price          // snapshot price at order time
                }).ToList();

                var order = new Order
                {
                    UserId = dto.UserId,
                    orderDate = DateTime.UtcNow,
                    OrderProducts = orderProducts
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Order placed successfully | OrderID: {OrderId} | UserID: {UserId} | Total: {Total}",
                    order.order_Id, order.UserId, order.TotalAmount);

                return new OrderOutput
                {
                    order_Id = order.order_Id,
                    orderDate = order.orderDate,
                    totalAmount = order.TotalAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error placing order for UserID: {UserId}", dto.UserId);
                return null;
            }
        }

        // ─────────────────────────────────────────────
        // GET ORDERS BY USER
        // ─────────────────────────────────────────────
        public async Task<List<OrderOutput>> GetOrdersByUser(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .OrderByDescending(o => o.orderDate)
                .Select(o => new OrderOutput
                {
                    order_Id = o.order_Id,
                    orderDate = o.orderDate,
                    totalAmount = o.OrderProducts!
                                   .Sum(op => op.Quantity * op.Price)
                })
                .ToListAsync();
        }

        // ─────────────────────────────────────────────
        // GET SINGLE ORDER
        // ─────────────────────────────────────────────
        public async Task<Order?> GetOrderById(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.order_Id == orderId);
        }
    }
}
