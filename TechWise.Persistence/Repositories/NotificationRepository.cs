using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class NotificationRepository(StoreDbContext _context)
        : INotificationRepository
    {
        public async Task AddAsync(Notification notification)
        {
            _context.ChangeTracker.Clear();

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Product)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId
                                       && n.UserId == userId);
            if (notification is null) return;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId
                                       && n.UserId == userId);
            if (notification is null) return;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}