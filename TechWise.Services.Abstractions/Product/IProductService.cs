using TechWise.Shared.DTOs.Products;

namespace TechWise.Services.Abstractions.Products
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductCardResponse>> GetProductsAsync(
            ProductFilterRequest request);

        Task<ProductDetailsResponse?> GetProductByIdAsync(int id);

        Task<List<string>> GetCategoriesAsync();


        // Recommendation
        Task<List<RecommendationResultResponse>> GetUserRecommendationsAsync(
            string userId, string? operationType);
        Task<RecommendationDetailsResponse?> GetRecommendationDetailsAsync(
            string userId, int productId);

        // Saved Items
        Task<List<SavedItemResponse>> GetSavedItemsAsync(string userId);
        Task SaveProductAsync(string userId, int productId);
        Task UnsaveProductAsync(string userId, int productId);
        Task<bool> IsProductSavedAsync(string userId, int productId);

    }
}