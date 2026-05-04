using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(string userId, int orderId);
        Task UpdateStatusAsync(Order order);

        Task<Order?> GetOrderByPaymentIntentAsync(string paymentIntentId);
    }
}