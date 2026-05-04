using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task AddRangeAsync(List<Notification> notifications);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteAsync(int notificationId, string userId);
    }
}