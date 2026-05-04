using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Cart;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.CartService.GetCartAsync(userId);
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.CartService
                .AddToCartAsync(userId, request);
            return Ok(result);
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(
            int productId, [FromBody] UpdateCartItemRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.CartService
                .UpdateCartItemAsync(userId, productId, request);
            return Ok(result);
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.CartService.RemoveFromCartAsync(userId, productId);
            return Ok(new { Message = "Item removed from cart" });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.CartService.ClearCartAsync(userId);
            return Ok(new { Message = "Cart cleared" });
        }
    }
}