using TechWise.Shared.DTOs.Orders;

namespace TechWise.Services.Abstractions.Orders
{
    public interface IOrderService
    {
        Task<OrderResponse> CheckoutAsync(string userId, CheckoutRequest request);
        Task<List<OrderResponse>> GetUserOrdersAsync(string userId);
        Task<OrderResponse?> GetOrderByIdAsync(string userId, int orderId);

        Task HandleStripeWebhookAsync(string json, string stripeSignature);

    }
}