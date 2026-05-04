using TechWise.Domains.Entities.Support;
using TechWise.Shared.DTOs.Admin;
using TechWise.Shared.DTOs.Notification;

namespace TechWise.Services.Abstractions.Admin
{
    public interface IAdminService
    {
        // Stats
        Task<AdminStatsResponse> GetStatsAsync();

        // Products
        Task<PaginatedAdminResponse<AdminProductResponse>> GetProductsAsync(
            string? category, string? brand, int pageNumber, int pageSize);
        Task<AdminProductResponse> UpdateProductAsync(
            int id, UpdateProductRequest request);
        Task DeleteProductAsync(int id);

        // Orders
        Task<PaginatedAdminResponse<AdminOrderResponse>> GetOrdersAsync(
            int pageNumber, int pageSize);
        Task<AdminOrderResponse> GetOrderByIdAsync(int orderId);
        Task<AdminOrderResponse> UpdateOrderStatusAsync(
            int orderId, UpdateOrderStatusRequest request);

        // Users
        Task<List<AdminUserResponse>> GetUsersAsync();
        Task DeleteUserAsync(string userId);

        // Reviews
        Task<PaginatedAdminResponse<AdminReviewResponse>> GetReviewsAsync(
            int pageNumber, int pageSize);
        Task DeleteReviewAsync(int reviewId);

        // Support
        Task<List<ContactMessage>> GetContactMessagesAsync();
        Task MarkContactAsReadAsync(int messageId);
        Task<List<Feedback>> GetFeedbacksAsync();

        // Market Alert
        Task<List<MarketAlertResponse>> GetMarketAlertsAsync();

    }
}