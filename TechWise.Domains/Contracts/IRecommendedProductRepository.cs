using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IRecommendedProductRepository
    {
        Task<List<RecommendedProduct>> GetUserRecommendationsAsync(
            string userId, string? operationType);

        Task<RecommendedProduct?> GetRecommendedProductDetailsAsync(
            string userId, int productId, int userReqNumber);


        Task<RecommendedProduct?> GetLatestRecommendationAsync(
            string userId, int productId);

    }
}