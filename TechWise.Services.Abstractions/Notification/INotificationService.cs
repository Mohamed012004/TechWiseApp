using TechWise.Shared.DTOs.Notification;
using TechWise.Shared.DTOs.Notifications;

namespace TechWise.Services.Abstractions.Notifications
{
    public interface INotificationService
    {
        // User
        Task<List<NotificationResponse>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(string userId, int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteNotificationAsync(string userId, int notificationId);

        // Settings
        Task<NotificationSettingsResponse> GetSettingsAsync(string userId);
        Task<NotificationSettingsResponse> UpdateSettingsAsync(
            string userId, UpdateNotificationSettingsRequest request);

        // Send
        Task SendOrderUpdateAsync(string userId, int orderId, string status);
        Task SendMarketAlertAsync(
            string adminId, CreateMarketAlertRequest request);
    }
}