using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IAdminRepository
    {
        // Stats
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTodayOrdersAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalReviewsAsync();

        // Products
        Task<List<Product>> GetAllProductsAsync(
            string? category, string? brand, int pageNumber, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);

        // Orders
        Task<List<Order>> GetAllOrdersAsync(int pageNumber, int pageSize);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderAsync(Order order);

        // Reviews
        Task<List<Review>> GetAllReviewsAsync(int pageNumber, int pageSize);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task DeleteReviewAsync(Review review);
    }
}