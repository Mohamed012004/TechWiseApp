// NotificationService.cs
using Microsoft.AspNetCore.SignalR;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;
using TechWise.Services.Abstractions.Notifications;
using TechWise.Shared.DTOs.Notification;
using TechWise.Shared.DTOs.Notifications;
using TechWise.Shared.Enums;
using TechWiseApp.Web.Hubs;

namespace TechWise.Services.Notifications
{
    public class NotificationService(
        IUnitOfWork _unitOfWork,
        IHubContext<NotificationHub> _hubContext
    ) : INotificationService
    {
        // ===== User =====
        public async Task<List<NotificationResponse>> GetUserNotificationsAsync(
            string userId)
        {
            var notifications = await _unitOfWork.Notifications
                .GetUserNotificationsAsync(userId);
            return notifications.Select(MapToResponse).ToList();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _unitOfWork.Notifications.GetUnreadCountAsync(userId);

        public async Task MarkAsReadAsync(string userId, int notificationId)
            => await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId);

        public async Task MarkAllAsReadAsync(string userId)
            => await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);

        public async Task DeleteNotificationAsync(string userId, int notificationId)
            => await _unitOfWork.Notifications.DeleteAsync(notificationId, userId);

        // ===== Settings =====
        public async Task<NotificationSettingsResponse> GetSettingsAsync(string userId)
        {
            var settings = await _unitOfWork.NotificationSettings
                .GetOrCreateAsync(userId);
            return MapToSettingsResponse(settings);
        }

        public async Task<NotificationSettingsResponse> UpdateSettingsAsync(
            string userId, UpdateNotificationSettingsRequest request)
        {
            var settings = await _unitOfWork.NotificationSettings
                .GetOrCreateAsync(userId);

            settings.PushNotifications = request.PushNotifications;
            settings.EmailUpdates = request.EmailUpdates;
            settings.PriceDropAlerts = request.PriceDropAlerts;

            await _unitOfWork.NotificationSettings.UpdateAsync(settings);
            return MapToSettingsResponse(settings);
        }

        // ===== Order Update =====
        public async Task SendOrderUpdateAsync(
            string userId, int orderId, string status)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = "Order Update",
                Message = $"Your order #{orderId} is now {status}",
                Type = NotificationType.OrderUpdate,
                OrderId = orderId
            };

            // Store in DB
            await _unitOfWork.Notifications.AddAsync(notification);

            // send Real-time if user Online
            await _hubContext.Clients.Group(userId)
                .SendAsync("ReceiveNotification", MapToResponse(notification));
        }

        // ===== Market Alert =====
        public async Task SendMarketAlertAsync(
            string adminId, CreateMarketAlertRequest request)
        {
            // store the Alert
            var alert = new MarketAlert
            {
                Title = request.Title,
                CreatedByAdminId = adminId,
                Products = request.ProductIds.Select(pid =>
                    new MarketAlertProduct { ProductId = pid }).ToList()
            };

            await _unitOfWork.MarketAlerts.CreateAsync(alert);

            // determine users those make Push Notifications
            var userIds = await _unitOfWork.MarketAlerts
                .GetUsersWithPushEnabledAsync();

            // make Notification for each user
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = request.Title,
                Message = $"{request.ProductIds.Count} new deals available!",
                Type = NotificationType.MarketAlert,
                ProductId = request.ProductIds.FirstOrDefault()
            }).ToList();

            await _unitOfWork.Notifications.AddRangeAsync(notifications);

            // send Real-time for user Online
            foreach (var userId in userIds)
            {
                await _hubContext.Clients.Group(userId)
                    .SendAsync("ReceiveNotification", new
                    {
                        title = request.Title,
                        message = $"{request.ProductIds.Count} new deals available!",
                        type = "MarketAlert"
                    });
            }
        }

        // ===== Helpers =====
        private static NotificationResponse MapToResponse(Notification n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            ProductId = n.ProductId,
            OrderId = n.OrderId,
            CreatedAt = n.CreatedAt
        };

        private static NotificationSettingsResponse MapToSettingsResponse(
            NotificationSettings s) => new()
            {
                PushNotifications = s.PushNotifications,
                EmailUpdates = s.EmailUpdates,
                PriceDropAlerts = s.PriceDropAlerts
            };
    }
}