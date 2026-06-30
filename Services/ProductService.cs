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

        public async Task<List<ProductDOT>> GetProducts(
            string? name,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber,
            int pageSize)
        {
            try
            {
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

                return products.Select(p => new ProductDOT
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products.");
                return new List<ProductDOT>();
            }
        }

        public async Task<bool> DeleteProductById(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return false;

                // Optionally delete the image file from disk too
                if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    var path = Path.Combine("wwwroot", product.ImageUrl.TrimStart('/'));
                    if (File.Exists(path))
                        File.Delete(path);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", id);
                return false;
            }
        }

        public async Task<int> AddProduct(ProductDOT input)
        {
            try
            {
                var product = new Product
                {
                    Name = input.Name,
                    Description = input.Description,
                    Price = input.Price,
                    Stock = input.Stock,
                    ImageUrl = input.ImageUrl
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product: {Name}", input.Name);
                return 0;
            }
        }

        public async Task<ProductDOT?> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return null;

                return new ProductDOT
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product with ID: {ProductId}", id);
                return null;
            }
        }

        public async Task<bool> UpdateProduct(ProductDOT input)
        {
            try
            {
                var product = await _context.Products.FindAsync(input.Id);
                if (product == null) return false;

                product.Name = input.Name;
                product.Description = input.Description;
                product.Price = input.Price;
                product.Stock = input.Stock;

                // Only overwrite the image if a new one was uploaded
                if (!string.IsNullOrWhiteSpace(input.ImageUrl))
                    product.ImageUrl = input.ImageUrl;

                await _context.SaveChangesAsync();
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