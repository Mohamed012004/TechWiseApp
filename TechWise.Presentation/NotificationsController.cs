using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Notification;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController(IServiceManager _serviceManager)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.NotificationService
                .GetUserNotificationsAsync(userId);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var count = await _serviceManager.NotificationService
                .GetUnreadCountAsync(userId);
            return Ok(new { UnreadCount = count });
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.NotificationService
                .MarkAsReadAsync(userId, notificationId);
            return Ok(new { Message = "Marked as read" });
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.NotificationService.MarkAllAsReadAsync(userId);
            return Ok(new { Message = "All marked as read" });
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.NotificationService
                .DeleteNotificationAsync(userId, notificationId);
            return Ok(new { Message = "Notification deleted" });
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.NotificationService
                .GetSettingsAsync(userId);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(
            [FromBody] UpdateNotificationSettingsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.NotificationService
                .UpdateSettingsAsync(userId, request);
            return Ok(result);
        }
    }
}