using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("{productId}")]
        public async Task<IActionResult> SaveProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.ProductService.SaveProductAsync(userId, productId);
            return Ok(new { Message = "Product saved successfully" });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> UnsaveProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.ProductService.UnsaveProductAsync(userId, productId);
            return Ok(new { Message = "Product removed from favorites" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ProductService.GetSavedItemsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{productId}/is-saved")]
        public async Task<IActionResult> IsFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ProductService
                .IsProductSavedAsync(userId, productId);
            return Ok(new { IsSaved = result });
        }
    }
}