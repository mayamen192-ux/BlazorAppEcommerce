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

        public int PlaceOrder(PlaceOrderDTO dto)
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

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                foreach (var item in dto.Items)
                {
                    var product = _productService.GetProductById(item.Pid);

                    if (product == null)
                    {
                        throw new Exception($"Product with Id {item.Pid} not found.");
                    }

                    if (!product.Stock.HasValue || product.Stock.Value < item.qnt)
                    {
                        throw new Exception(
                            $"Insufficient stock for product '{product.Name}'.");
                    }

                    product.Stock = product.Stock.Value - item.qnt;

                    order.OrderProducts.Add(new OrderProducts
                    {
                        ProductId = product.Id,
                        Quantity = item.qnt,
                        Price = product.Price
                    });
                }

                int orderId = _orderService.AddOrder(order);

                _context.SaveChanges();

                transaction.Commit();

                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}