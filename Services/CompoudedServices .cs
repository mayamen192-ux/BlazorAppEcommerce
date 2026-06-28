using BlazorAppEcommerce.DTOs;
using BlazorAppEcommerce.Models;

namespace BlazorAppEcommerce.Services
{
    public class CompoudedServices
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly ApplicationDbContext _context;

        public CompoudedServices(
            ProductService productService,
            OrderService orderService,
            ApplicationDbContext context)
        {
            _productService = productService;
            _orderService = orderService;
            _context = context;
        }

        public async Task<OrderOutput?> PlaceOrder(PlaceOrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ✅ Validate stock and decrement for each item
                foreach (var item in dto.Items)
                {
                    var product = await _productService.GetProductById(item.ProductId);

                    if (product == null)
                        throw new Exception($"Product with Id {item.ProductId} not found.");

                    if (product.Stock < item.Quantity)
                        throw new Exception($"Insufficient stock for product '{product.Name}'.");

                    product.Stock -= item.Quantity;
                }

                // ✅ Delegate order creation to OrderService (it accepts PlaceOrderDTO)
                var result = await _orderService.AddOrder(dto);

                if (result == null)
                    throw new Exception("Order could not be placed.");

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}