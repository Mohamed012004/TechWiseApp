
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Cart;
using TechWise.Shared.DTOs.Cart;

namespace TechWise.Services.Carts
{
    public class CartService(IUnitOfWork _unitOfWork) : ICartService
    {
        public async Task<CartResponse> GetCartAsync(string userId)
        {
            var cart = await _unitOfWork.Cart.GetOrCreateCartAsync(userId);
            return MapToResponse(cart);
        }

        public async Task<CartResponse> AddToCartAsync(
            string userId, AddToCartRequest request)
        {
            var product = await _unitOfWork.Products
                .GetProductByIdAsync(request.ProductId);
            if (product is null) throw new ProductNotFoundException(request.ProductId);

            var cart = await _unitOfWork.Cart.GetOrCreateCartAsync(userId);

            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem is not null)
                existingItem.Quantity += request.Quantity;
            else
                cart.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Product = product
                });

            await _unitOfWork.Cart.UpdateAsync(cart);
            return MapToResponse(cart);
        }

        public async Task<CartResponse> UpdateCartItemAsync(
            string userId, int productId, UpdateCartItemRequest request)
        {
            var cart = await _unitOfWork.Cart.GetOrCreateCartAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) throw new ProductNotFoundException(productId);

            if (request.Quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = request.Quantity;

            await _unitOfWork.Cart.UpdateAsync(cart);
            return MapToResponse(cart);
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _unitOfWork.Cart.GetOrCreateCartAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;

            cart.Items.Remove(item);
            await _unitOfWork.Cart.UpdateAsync(cart);
        }

        public async Task ClearCartAsync(string userId)
            => await _unitOfWork.Cart.ClearCartAsync(userId);

        private static CartResponse MapToResponse(Cart cart)
        {
            return new CartResponse
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemResponse
                {
                    ProductId = i.ProductId,
                    Title = i.Product.Title,
                    Brand = i.Product.Brand,
                    Category = i.Product.Category,
                    ImageUrl = i.Product.ImageUrl,
                    Price = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}