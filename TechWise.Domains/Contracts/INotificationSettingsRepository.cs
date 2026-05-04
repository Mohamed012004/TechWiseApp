using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface INotificationSettingsRepository
    {
        Task<NotificationSettings> GetOrCreateAsync(string userId);
        Task UpdateAsync(NotificationSettings settings);
    }
}