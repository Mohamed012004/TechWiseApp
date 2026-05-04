
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Domains.Exceptions.UnAuthorized;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Profile;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await _serviceManager.ProfileService.GetProfileAsync(email);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await _serviceManager.ProfileService
                .UpdateProfileAsync(email, request);
            return Ok(result);
        }

        [HttpPut("photo")]
        public async Task<IActionResult> UpdatePhoto(IFormFile photo)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;

            using var stream = photo.OpenReadStream();
            var result = await _serviceManager.ProfileService
                .UpdateProfilePhotoAsync(email, stream, photo.FileName);

            return Ok(result);
        }


        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email is null) throw new UnAuthorizedException();

            await _serviceManager.ProfileService.DeleteAccountAsync(email);
            return Ok(new { Message = "Account deleted successfully" });
        }


    }
}