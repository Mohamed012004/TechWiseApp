using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWise.Domains.Entities.SuperAdmin;
using TechWise.Services.Abstractions;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var result = await _serviceManager.SuperAdminService.GetAdminsAsync();
            return Ok(result);
        }

        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdmin(
            [FromBody] CreateAdminRequest request)
        {
            var result = await _serviceManager.SuperAdminService
                .CreateAdminAsync(request);
            return Ok(result);
        }

        [HttpDelete("admins/{adminId}")]
        public async Task<IActionResult> DeleteAdmin(string adminId)
        {
            await _serviceManager.SuperAdminService.DeleteAdminAsync(adminId);
            return Ok(new { Message = "Admin deleted successfully" });
        }
    }
}