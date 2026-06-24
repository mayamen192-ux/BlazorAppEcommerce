using BlazorAppEcommerce;
using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEcommerce.Services
{
    public class ProductImagesService
    {
        private readonly ApplicationDbContext _context;

        public ProductImagesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int AddImages(List<ProductImage> images)
        {
            _context.ProductImages.AddRange(images);
            return _context.SaveChanges();
        }

        public ProductImage? GetFirstImages(int prid)
        {
            return _context.ProductImages
                .FirstOrDefault(x => x.ProductId == prid);
        }

        public IEnumerable<ProductImage> GetImages(int prid)
        {
            return _context.ProductImages
                .Where(x => x.ProductId == prid)
                .ToList();
        }
    }
}