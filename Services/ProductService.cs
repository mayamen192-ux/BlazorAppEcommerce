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

        public int AddProduct(ProductDOT input)
        {
            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Stock = input.Stock ?? 0,
                Price = input.Price,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return product.Id;
        }

        public List<ProductDOT> GetProducts(string? name, decimal? minPrice, decimal? maxPrice, int pageNumber, int pageSize)
        {
            var query = _context.Products.Include(p => p.Reviews).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(product => new ProductDOT
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock
                })
                .ToList();
        }

        public ProductDOT? GetProductById(int id)
        {
            var product = _context.Products.Include(p => p.Reviews).FirstOrDefault(p => p.Id == id);
            if (product == null) return null;

            return new ProductDOT
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };
        }
        public void UpdateProduct(ProductDOT updatedProduct)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == updatedProduct.Id);

            if (product == null)
                return;

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;

            _context.SaveChanges();
        }
        public bool DeleteProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
    }
}