using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;
using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Reviews;
using TechWise.Shared.DTOs.Reviews;

namespace TechWise.Services.Reviews
{
    public class ReviewService(IUnitOfWork _unitOfWork) : IReviewService
    {
        public async Task<ReviewResponse> AddReviewAsync(
            string userId, int productId, ReviewRequest request)
        {
            // check if product exists
            var product = await _unitOfWork.Products.GetProductByIdAsync(productId);
            if (product is null) throw new ProductNotFoundException(productId);

            // check if user has already reviewed this product
            var hasReviewed = await _unitOfWork.Reviews
                .HasReviewedAsync(userId, productId);
            if (hasReviewed)
                throw new BadRequestException(
                    "You have already reviewed this product");

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await _unitOfWork.Reviews.AddAsync(review);
            return MapToResponse(review);
        }

        public async Task<ReviewResponse> UpdateReviewAsync(
            string userId, int productId, ReviewRequest request)
        {
            var review = await _unitOfWork.Reviews
                .GetReviewAsync(userId, productId);

            if (review is null)
                throw new ReviewNotFoundException();

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.UpdateAsync(review);
            return MapToResponse(review);
        }

        public async Task DeleteReviewAsync(string userId, int productId)
        {
            var review = await _unitOfWork.Reviews
                .GetReviewAsync(userId, productId);

            if (review is null)
                throw new ReviewNotFoundException();

            await _unitOfWork.Reviews.DeleteAsync(review);
        }

        public async Task<ProductReviewsResponse> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews
                .GetProductReviewsAsync(productId);

            return new ProductReviewsResponse
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.Rating), 1)
                    : 0,
                Reviews = reviews.Select(MapToResponse).ToList()
            };
        }

        public async Task<List<ReviewResponse>> GetUserReviewsAsync(string userId)
        {
            var reviews = await _unitOfWork.Reviews.GetUserReviewsAsync(userId);
            return reviews.Select(MapToResponse).ToList();
        }

        private static ReviewResponse MapToResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
}