using TechWise.Shared.DTOs.Auth;
using TechWise.Shared.DTOs.Auth.ExternalAuthServer;
using TechWise.Shared.DTOs.Auth.NormalAuthServer;

public interface IAuthService
{
    Task<UserResponse?> LoginAsync(LoginRequest request);
    Task<string> RegisterAsync(RegisterRequest request);

    Task SendResetCodeAsync(ForgetPasswordRequest request);
    Task<VerifyResetCodeResponse> VerifyResetCodeAsync(VerifyResetCodeRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);

    Task ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<UserResponse> GoogleLoginAsync(SocialLoginRequest request);
    Task<UserResponse> FacebookLoginAsync(SocialLoginRequest request);




    Task SendVerificationCodeAsync(string email);
    Task<UserResponse> VerifyEmailAsync(VerifyEmailRequest request);

}