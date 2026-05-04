using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Admin;
using TechWise.Shared.DTOs.Notifications;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _serviceManager.AdminService.GetStatsAsync();
            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
           [FromQuery] string? category,
           [FromQuery] string? brand,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.AdminService
                .GetProductsAsync(category, brand, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id, [FromBody] UpdateProductRequest request)
        {
            var result = await _serviceManager.AdminService
                .UpdateProductAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _serviceManager.AdminService.DeleteProductAsync(id);
            return Ok(new { Message = "Product deleted successfully" });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.AdminService
                .GetOrdersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _serviceManager.AdminService.GetOrderByIdAsync(orderId);
            return Ok(result);
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _serviceManager.AdminService
                .UpdateOrderStatusAsync(orderId, request);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _serviceManager.AdminService.GetUsersAsync();
            return Ok(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _serviceManager.AdminService.DeleteUserAsync(userId);
            return Ok(new { Message = "User deleted successfully" });
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.AdminService
                .GetReviewsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            await _serviceManager.AdminService.DeleteReviewAsync(reviewId);
            return Ok(new { Message = "Review deleted successfully" });
        }


        [HttpGet("contact-messages")]
        public async Task<IActionResult> GetContactMessages()
        {
            var result = await _serviceManager.AdminService.GetContactMessagesAsync();
            return Ok(result);
        }

        [HttpPut("contact-messages/{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _serviceManager.AdminService.MarkContactAsReadAsync(id);
            return Ok(new { Message = "Marked as read" });
        }

        [HttpGet("feedbacks")]
        public async Task<IActionResult> GetFeedbacks()
        {
            var result = await _serviceManager.AdminService.GetFeedbacksAsync();
            return Ok(result);
        }

        // Notifications and Market Alerts
        [HttpPost("market-alerts")]
        public async Task<IActionResult> SendMarketAlert(
        [FromBody] CreateMarketAlertRequest request)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.NotificationService
                .SendMarketAlertAsync(adminId, request);
            return Ok(new { Message = "Market Alert sent successfully" });
        }


        [HttpGet("market-alerts")]
        public async Task<IActionResult> GetMarketAlerts()
        {
            var result = await _serviceManager.AdminService.GetMarketAlertsAsync();
            return Ok(result);
        }



    }
}