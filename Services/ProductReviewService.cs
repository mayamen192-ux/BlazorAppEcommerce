using BlazorAppEcommerce;
using BlazorAppEcommerce.Models;
using Microsoft.Extensions.Logging;

namespace BlazorAppEcommerce.Services
{
    public class ProductReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductReviewService> _logger;

        public ProductReviewService(ApplicationDbContext context, ILogger<ProductReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================
        // ADD REVIEW
        // =====================
        public int AddReview(Review review)
        {
            try
            {
                _logger.LogInformation(
                    "Adding new review | ProductID: {ProductId} | UserID: {UserId} | Rating: {Rating}",
                    review.ProductId, review.UserId, review.Rating);

                _context.Reviews.Add(review);
                var result = _context.SaveChanges();

                _logger.LogInformation(
                    "Review added successfully | ProductID: {ProductId} | UserID: {UserId}",
                    review.ProductId, review.UserId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while adding review for ProductID: {ProductId} | UserID: {UserId}",
                    review.ProductId, review.UserId);

                return 0;
            }
        }

        // =====================
        // GET REVIEWS BY PRODUCT ID
        // =====================
        public List<Review> GetReviewsById(int pId)
        {
            try
            {
                _logger.LogInformation("Fetching reviews for ProductID: {ProductId}", pId);

                var reviews = _context.Reviews
                    .Where(x => x.ProductId == pId)
                    .ToList();

                if (reviews.Count == 0)
                    _logger.LogWarning("No reviews found for ProductID: {ProductId}", pId);
                else
                    _logger.LogInformation(
                        "Successfully fetched {Count} reviews for ProductID: {ProductId}",
                        reviews.Count, pId);

                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching reviews for ProductID: {ProductId}", pId);

                return new List<Review>();
            }
        }
    }
}