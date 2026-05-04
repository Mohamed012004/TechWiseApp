using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart> GetOrCreateCartAsync(string userId);
        Task UpdateAsync(Cart cart);
        Task ClearCartAsync(string userId);
    }
}