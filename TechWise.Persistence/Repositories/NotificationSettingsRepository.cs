// NotificationSettingsRepository.cs
using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class NotificationSettingsRepository(StoreDbContext _context)
        : INotificationSettingsRepository
    {
        public async Task<NotificationSettings> GetOrCreateAsync(string userId)
        {
            var settings = await _context.NotificationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (settings is not null) return settings;

            //Create Default Setting for New User
            settings = new NotificationSettings { UserId = userId };
            await _context.NotificationSettings.AddAsync(settings);
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task UpdateAsync(NotificationSettings settings)
        {
            _context.NotificationSettings.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}