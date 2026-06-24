using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEcommerce.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }
    }
}