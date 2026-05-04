using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IMarketAlertRepository
    {
        Task<MarketAlert> CreateAsync(MarketAlert alert);
        Task<List<MarketAlert>> GetAllAsync();
        Task<List<string>> GetUsersWithPushEnabledAsync();
    }
}