using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Products;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IServiceManager _serviceManager) : ControllerBase
    {
        // GET /api/products?category=cpu&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterRequest request)
        {
            var result = await _serviceManager.ProductService.GetProductsAsync(request);
            return Ok(result);
        }

        // GET /api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _serviceManager.ProductService.GetProductByIdAsync(id);
            return Ok(result);
        }

        // GET /api/products/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _serviceManager.ProductService.GetCategoriesAsync();
            return Ok(result);
        }


        // GET /api/products/recommendations?operationType=Recommendation
        [HttpGet("recommendations")]
        [Authorize]
        public async Task<IActionResult> GetRecommendations(
            [FromQuery] string? operationType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ProductService
                .GetUserRecommendationsAsync(userId, operationType);
            return Ok(result);
        }

        // GET /api/products/recommendations/{productId}
        [HttpGet("recommendations/{productId}")]
        [Authorize]
        public async Task<IActionResult> GetRecommendationDetails(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ProductService
                .GetRecommendationDetailsAsync(userId, productId);
            return Ok(result);
        }


    }
}