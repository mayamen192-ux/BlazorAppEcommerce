using BlazorAppEcommerce.DTOs;
using BlazorAppEcommerce.Models;

namespace BlazorAppEcommerce.Services
{
    public class CompoudedServices
    {
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly ApplicationDbContext _context;

        public CompoudedServices(
            UserService userService,
            ProductService productService,
            OrderService orderService,
            ApplicationDbContext context)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _context = context;
        }

        public async Task<int> PlaceOrder(PlaceOrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            var order = new Order
            {
                UserId = dto.UserId,
                orderDate = DateTime.Now,
                OrderProducts = new List<OrderProducts>()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in dto.Items)
                {
                    var product = await _productService.GetProductById(item.Pid);

                    if (product == null)
                        throw new Exception($"Product with Id {item.Pid} not found.");

                    // ✅ FIXED: no HasValue / Value
                    if (product.Stock < item.qnt)
                        throw new Exception($"Insufficient stock for product '{product.Name}'.");

                    product.Stock -= item.qnt;

                    order.OrderProducts.Add(new OrderProducts
                    {
                        ProductId = product.Id,
                        Quantity = item.qnt,
                        Price = product.Price
                    });
                }

                var orderId = await _orderService.AddOrder(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return orderId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}