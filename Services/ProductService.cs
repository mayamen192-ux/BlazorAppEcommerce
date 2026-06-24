using BlazorAppEcommerce.DTOs;
using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazorAppEcommerce.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================
        // GET PRODUCTS
        // =====================
        public async Task<List<ProductDOT>> GetProducts(
            string? name,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber,
            int pageSize)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching products | Name: {Name} | MinPrice: {MinPrice} | MaxPrice: {MaxPrice} | Page: {Page} | PageSize: {PageSize}",
                    name, minPrice, maxPrice, pageNumber, pageSize);

                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(p => p.Name.Contains(name));

                if (minPrice.HasValue)
                    query = query.Where(p => p.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(p => p.Price <= maxPrice.Value);

                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} products.", products.Count);

                return products.Select(p => new ProductDOT
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products.");
                return new List<ProductDOT>();
            }
        }

        // =====================
        // DELETE
        // =====================
        public async Task<bool> DeleteProductById(int id)
        {
            try
            {
                _logger.LogWarning("Attempting to delete product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found. Delete aborted.", id);
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
                return false;
            }
        }

        // =====================
        // ADD
        // =====================
        public async Task<int> AddProduct(ProductDOT input)
        {
            try
            {
                _logger.LogInformation("Adding new product | Name: {Name} | Price: {Price} | Stock: {Stock}",
                    input.Name, input.Price, input.Stock);

                var product = new Product
                {
                    Name = input.Name,
                    Description = input.Description,
                    Price = input.Price,
                    Stock = input.Stock
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product added successfully | ID: {ProductId} | Name: {Name}",
                    product.Id, product.Name);

                return product.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product: {Name}", input.Name);
                return 0;
            }
        }

        // =====================
        // GET BY ID
        // =====================
        public async Task<ProductDOT?> GetProductById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", id);
                    return null;
                }

                _logger.LogInformation("Product with ID {ProductId} fetched successfully.", id);

                return new ProductDOT
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product with ID: {ProductId}", id);
                return null;
            }
        }

        // =====================
        // UPDATE
        // =====================
        public async Task<bool> UpdateProduct(ProductDOT input)
        {
            try
            {
                _logger.LogInformation("Updating product | ID: {ProductId} | Name: {Name} | Price: {Price} | Stock: {Stock}",
                    input.Id, input.Name, input.Price, input.Stock);

                var product = await _context.Products.FindAsync(input.Id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found. Update aborted.", input.Id);
                    return false;
                }

                product.Name = input.Name;
                product.Description = input.Description;
                product.Price = input.Price;
                product.Stock = input.Stock;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} updated successfully.", input.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", input.Id);
                return false;
            }
        }
    }
}