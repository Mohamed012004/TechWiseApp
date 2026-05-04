using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class MarketAlertRepository(StoreDbContext _context)
        : IMarketAlertRepository
    {
        public async Task<MarketAlert> CreateAsync(MarketAlert alert)
        {
            await _context.MarketAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<List<MarketAlert>> GetAllAsync()
        {
            return await _context.MarketAlerts
                .Include(ma => ma.Products)
                    .ThenInclude(mp => mp.Product)
                .OrderByDescending(ma => ma.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<string>> GetUsersWithPushEnabledAsync()
        {
            return await _context.NotificationSettings
                .Where(ns => ns.PushNotifications)
                .Select(ns => ns.UserId)
                .ToListAsync();
        }
    }
}