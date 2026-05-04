using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface ISavedItemRepository
    {
        Task<List<SavedItem>> GetUserSavedItemsAsync(string userId);
        Task<SavedItem?> GetSavedItemAsync(string userId, int productId);
        Task AddAsync(SavedItem savedItem);
        Task DeleteAsync(SavedItem savedItem);
        Task<bool> IsSavedAsync(string userId, int productId);
    }
}