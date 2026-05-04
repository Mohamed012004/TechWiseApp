using TechWise.Shared.DTOs.Reviews;

namespace TechWise.Services.Abstractions.Reviews
{
    public interface IReviewService
    {
        Task<ReviewResponse> AddReviewAsync(
            string userId, int productId, ReviewRequest request);

        Task<ReviewResponse> UpdateReviewAsync(
            string userId, int productId, ReviewRequest request);

        Task DeleteReviewAsync(string userId, int productId);

        Task<ProductReviewsResponse> GetProductReviewsAsync(int productId);

        Task<List<ReviewResponse>> GetUserReviewsAsync(string userId);
    }
}