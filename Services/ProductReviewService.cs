using BlazorAppEcommerce;
using BlazorAppEcommerce.Models;

namespace BlazorAppEcommerce.Services
{
    public class ProductReviewService
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int AddReview(Review review)
        {
            _context.Reviews.Add(review);
            return _context.SaveChanges();
        }

        public List<Review> GetReviewsById(int pId)
        {
            return _context.Reviews
                .Where(x => x.ProductId == pId)
                .ToList();
        }
    }
}