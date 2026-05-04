using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetProductReviewsAsync(int productId);
        Task<List<Review>> GetUserReviewsAsync(string userId);
        Task<Review?> GetReviewAsync(string userId, int productId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<bool> HasReviewedAsync(string userId, int productId);
    }
}