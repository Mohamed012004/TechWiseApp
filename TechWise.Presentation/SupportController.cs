using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Support;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController(IServiceManager _serviceManager) : ControllerBase
    {

        [HttpPost("contact")]
        public async Task<IActionResult> SendMessage([FromBody] ContactRequest request)
        {
            await _serviceManager.SupportService.SendContactMessageAsync(request);
            return Ok(new { Message = "Message sent successfully" });
        }


        [HttpPost("feedback")]
        [Authorize]
        public async Task<IActionResult> SendFeedback([FromBody] FeedbackRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.SupportService.SendFeedbackAsync(userId, request);
            return Ok(new { Message = "Feedback sent successfully" });
        }
    }
}
