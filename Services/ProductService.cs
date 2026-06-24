using BlazorAppEcommerce.DTOs;
using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEcommerce.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET PRODUCTS
        public async Task<List<ProductDOT>> GetProducts(
            string? name,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber,
            int pageSize)
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
                Stock = p.Stock
            }).ToList();
        }

        // DELETE
        public async Task<bool> DeleteProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // ADD
        public async Task<int> AddProduct(ProductDOT input)
        {
            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Price = input.Price,
                Stock = input.Stock   // ✅ FIXED (no ?? needed)
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        // GET BY ID
        public async Task<ProductDOT?> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return null;

            return new ProductDOT
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };
        }

        // UPDATE
        public async Task<bool> UpdateProduct(ProductDOT input)
        {
            var product = await _context.Products.FindAsync(input.Id);

            if (product == null)
                return false;

            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.Stock = input.Stock;   

            await _context.SaveChangesAsync();
            return true;
        }
    }
}