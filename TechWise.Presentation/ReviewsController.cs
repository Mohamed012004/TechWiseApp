using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Reviews;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController(IServiceManager _serviceManager) : ControllerBase
    {
        // GET /api/reviews/{productId}
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _serviceManager.ReviewService
                .GetProductReviewsAsync(productId);
            return Ok(result);
        }

        // GET /api/reviews/my-reviews
        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ReviewService
                .GetUserReviewsAsync(userId);
            return Ok(result);
        }

        // POST /api/reviews/{productId}
        [HttpPost("{productId}")]
        [Authorize]
        public async Task<IActionResult> AddReview(
            int productId, [FromBody] ReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ReviewService
                .AddReviewAsync(userId, productId, request);
            return Ok(result);
        }

        // PUT /api/reviews/{productId}
        [HttpPut("{productId}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(
            int productId, [FromBody] ReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.ReviewService
                .UpdateReviewAsync(userId, productId, request);
            return Ok(result);
        }

        // DELETE /api/reviews/{productId}
        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.ReviewService
                .DeleteReviewAsync(userId, productId);
            return Ok(new { Message = "Review deleted successfully" });
        }
    }
}