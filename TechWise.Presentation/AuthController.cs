using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Auth;
using TechWise.Shared.DTOs.Auth.ExternalAuthServer;
using TechWise.Shared.DTOs.Auth.NormalAuthServer;

namespace TechWise.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IServiceManager _serviceManager) : ControllerBase
    {

        [HttpPost("SignUp")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var message = await _serviceManager.AuthService.RegisterAsync(request);
            return Ok(new { Message = message });
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _serviceManager.AuthService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _serviceManager.AuthService.VerifyEmailAsync(request);
            return Ok(result);
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] string email)
        {
            await _serviceManager.AuthService.SendVerificationCodeAsync(email);
            return Ok(new { Message = "Verification code sent" });
        }


        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            await _serviceManager.AuthService.SendResetCodeAsync(request);
            return Ok(new { Message = "Reset code sent to your email" });
        }

        [HttpPost("VerifyResetCode")]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest request)
        {
            var result = await _serviceManager.AuthService.VerifyResetCodeAsync(request);
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _serviceManager.AuthService.ResetPasswordAsync(request);
            return Ok(new { Message = "Password reset successfully" });
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin(SocialLoginRequest request)
        {
            var result = await _serviceManager.AuthService.GoogleLoginAsync(request);
            return Ok(result);
        }


        [HttpPost("FacebookLogin")]
        public async Task<IActionResult> FacebookLogin(SocialLoginRequest request)
        {
            var result = await _serviceManager.AuthService.FacebookLoginAsync(request);
            return Ok(result);
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.AuthService.ChangePasswordAsync(userId, request);
            return Ok(new { Message = "Password changed successfully" });
        }

    }
}