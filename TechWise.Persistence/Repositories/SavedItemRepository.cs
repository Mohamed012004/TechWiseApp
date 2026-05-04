using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class SavedItemRepository(StoreDbContext _context) : ISavedItemRepository
    {
        public async Task<List<SavedItem>> GetUserSavedItemsAsync(string userId)
        {
            return await _context.SavedItems
                .Include(s => s.Product)
                    .ThenInclude(p => p.Specs)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SavedAt)
                .ToListAsync();
        }

        public async Task<SavedItem?> GetSavedItemAsync(string userId, int productId)
        {
            return await _context.SavedItems
                .Include(s => s.Product)
                    .ThenInclude(p => p.Specs)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId);
        }

        public async Task AddAsync(SavedItem savedItem)
        {
            await _context.SavedItems.AddAsync(savedItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SavedItem savedItem)
        {
            _context.SavedItems.Remove(savedItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSavedAsync(string userId, int productId)
        {
            return await _context.SavedItems
                .AnyAsync(s => s.UserId == userId && s.ProductId == productId);
        }
    }
}