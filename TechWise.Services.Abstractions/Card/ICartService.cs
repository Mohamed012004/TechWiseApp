using TechWise.Shared.DTOs.Cart;

namespace TechWise.Services.Abstractions.Cart
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(string userId);
        Task<CartResponse> AddToCartAsync(string userId, AddToCartRequest request);
        Task<CartResponse> UpdateCartItemAsync(string userId, int productId, UpdateCartItemRequest request);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }
}