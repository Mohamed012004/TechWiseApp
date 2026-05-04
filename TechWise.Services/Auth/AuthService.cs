using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;
using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Domains.Exceptions.UnAuthorized;
using TechWise.Services.Abstractions.Email;
using TechWise.Shared;
using TechWise.Shared.DTOs.Auth;
using TechWise.Shared.DTOs.Auth.ExternalAuthServer;
using TechWise.Shared.DTOs.Auth.NormalAuthServer;

public class AuthService(
    UserManager<AppUser> _userManager,
    IOptions<JWTOptions> _jwtOptions,
    IEmailService _emailService,
    IUnitOfWork _unitOfWork,
    IHttpClientFactory _httpClientFactory

) : IAuthService
{

    public async Task<UserResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new UserNotFoundException(request.Email);

        if (user.IsDeleted) throw new AccountDeletedException();

        var flag = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!flag) throw new UnAuthorizedException();
        return new UserResponse
        {
            useId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = request.Email,
            Token = await GenerateTokenAsync(user)
        };
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        // Search Old Account in DB
        var deletedUser = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.IsDeleted &&
                u.OriginalEmail == request.Email);

        if (deletedUser is not null)
        {
            // Restore old Account
            deletedUser.Email = request.Email;
            deletedUser.UserName = request.Email;
            deletedUser.NormalizedEmail = request.Email.ToUpper();
            deletedUser.NormalizedUserName = request.Email.ToUpper();
            deletedUser.FirstName = request.FirstName;
            deletedUser.LastName = request.LastName;
            deletedUser.IsTermsAccepted = request.IsTermsAccepted;
            deletedUser.IsDeleted = false;
            deletedUser.DeletedAt = null;
            deletedUser.OriginalEmail = null;
            deletedUser.EmailConfirmed = false;

            await _userManager.RemovePasswordAsync(deletedUser);
            await _userManager.AddPasswordAsync(deletedUser, request.Password);
            await _userManager.UpdateSecurityStampAsync(deletedUser);
            await _userManager.UpdateAsync(deletedUser);

            // send Verification Code
            await SendVerificationCodeAsync(request.Email);

            return "Account restored. Please check your email to verify.";
        }

        // check if email Already Exist
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new RegistrationBadRequestException(
                new List<string> { "Email already exists" });

        // Create New Account
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsTermsAccepted = request.IsTermsAccepted,
            EmailConfirmed = false  // Not Confirmed until verify email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new RegistrationBadRequestException(
                result.Errors.Select(e => e.Description).ToList());

        //send Verification Code
        await SendVerificationCodeAsync(request.Email);

        // return Message Not Token because we need to verify email first
        return "Registration successful. Please check your email to verify.";
    }


    public async Task SendResetCodeAsync(ForgetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new UserNotFoundException(request.Email);

        await _unitOfWork.PasswordResetCodes.DeleteOldCodesAsync(request.Email);

        var code = new Random().Next(100000, 999999).ToString();

        await _unitOfWork.PasswordResetCodes.AddAsync(new PasswordResetCode
        {
            Email = request.Email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        await _unitOfWork.SaveChangesAsync();

        var body = $@"
            <div style='font-family:Arial; max-width:500px; margin:auto;'>
                <h2 style='color:#1E3A5F;'>TechWise - Reset Password</h2>
                <p>Your verification code is:</p>
                <div style='background:#F0F4FF; padding:20px; text-align:center; 
                            border-radius:8px; letter-spacing:12px;'>
                    <h1 style='color:#3B82F6; font-size:36px; margin:0;'>{code}</h1>
                </div>
                <p style='color:#666;'>
                    This code expires in <strong>10 minutes</strong>.
                </p>
            </div>";

        await _emailService.SendEmailAsync(
            request.Email,
            "TechWise - Password Reset Code",
            body);
    }
    public async Task<VerifyResetCodeResponse> VerifyResetCodeAsync(VerifyResetCodeRequest request)
    {
        var resetCode = await _unitOfWork.PasswordResetCodes
            .GetValidCodeAsync(request.Code);

        if (resetCode is null || resetCode.IsExpired)
            throw new InvalidResetCodeException();

        // Generate ResetToken Temporary
        var resetToken = Guid.NewGuid().ToString();
        resetCode.ResetToken = resetToken;
        await _unitOfWork.SaveChangesAsync();

        return new VerifyResetCodeResponse { ResetToken = resetToken };
    }
    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var resetCode = await _unitOfWork.PasswordResetCodes
            .GetByResetTokenAsync(request.ResetToken);

        if (resetCode is null || resetCode.IsExpired)
            throw new InvalidResetCodeException();

        var user = await _userManager.FindByEmailAsync(resetCode.Email);
        if (user is null) throw new UserNotFoundException(resetCode.Email);

        await _userManager.RemovePasswordAsync(user);
        var result = await _userManager.AddPasswordAsync(user, request.NewPassword);

        if (!result.Succeeded)
            throw new RegistrationBadRequestException(
                result.Errors.Select(e => e.Description).ToList());

        resetCode.IsUsed = true;
        await _unitOfWork.SaveChangesAsync();
    }



    public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) throw new UserNotFoundException(userId);

        //  varify to Current Password
        var isCorrect = await _userManager.CheckPasswordAsync(
            user, request.CurrentPassword);
        if (!isCorrect)
            throw new BadRequestException("Current password is incorrect");

        // change Password
        var result = await _userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        // send Email confirmation
        await _emailService.SendEmailAsync(
            user.Email!,
            "Password Changed - TechWise",
            $"""
        <h2>Password Changed Successfully</h2>
        <p>Hi {user.FirstName},</p>
        <p>Your password has been changed successfully.</p>
        <p>If you didn't make this change, please contact us immediately.</p>
        """
        );
    }



    public async Task<UserResponse> GoogleLoginAsync(SocialLoginRequest request)
    {
        // 1. varivy from Google Token
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch
        {
            throw new UnAuthorizedException();
        }

        // 2. Extract Data From Token
        var email = payload.Email;
        var firstName = payload.GivenName ?? email;
        var lastName = payload.FamilyName ?? "";

        // 3. search user in DB
        var user = await _userManager.FindByEmailAsync(email);

        // 4.Create User if Not Exist
        if (user is null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsTermsAccepted = true,
                EmailConfirmed = true  //  Confirmed Google Mail
            };

            // Create  Random Password 
            var randomPassword = $"Tw@{Guid.NewGuid().ToString("N")[..10]}";
            var result = await _userManager.CreateAsync(user, randomPassword);

            if (!result.Succeeded)
                throw new RegistrationBadRequestException(
                    result.Errors.Select(e => e.Description).ToList());
        }

        // 5. return our JWT
        return new UserResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Token = await GenerateTokenAsync(user)
        };
    }

    public async Task<UserResponse> FacebookLoginAsync(SocialLoginRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var fbUrl = $"https://graph.facebook.com/me" +
                    $"?fields=id,email,first_name,last_name" +
                    $"&access_token={request.IdToken}";

        var response = await httpClient.GetAsync(fbUrl);

        if (!response.IsSuccessStatusCode)
            throw new UnAuthorizedException();

        var content = await response.Content.ReadAsStringAsync();
        var fbUser = JsonSerializer.Deserialize<FacebookUserInfo>(content);

        if (fbUser?.Email is null)
            throw new BadRequestException("Facebook account must have an email");

        var user = await _userManager.FindByEmailAsync(fbUser.Email);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = fbUser.Email,
                Email = fbUser.Email,
                FirstName = fbUser.FirstName ?? fbUser.Email,
                LastName = fbUser.LastName ?? "",
                IsTermsAccepted = true,
                EmailConfirmed = true
            };

            var randomPassword = $"Tw@{Guid.NewGuid().ToString("N")[..10]}";
            var result = await _userManager.CreateAsync(user, randomPassword);

            if (!result.Succeeded)
                throw new RegistrationBadRequestException(
                    result.Errors.Select(e => e.Description).ToList());
        }

        return new UserResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Token = await GenerateTokenAsync(user)
        };
    }


    public async Task SendVerificationCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) throw new UserNotFoundException(email);

        // Remove Old Codes
        await _unitOfWork.EmailVerificationCodes.DeleteOldCodesAsync(email);

        // create new code
        var code = new Random().Next(100000, 999999).ToString();

        await _unitOfWork.EmailVerificationCodes.AddAsync(
            new EmailVerificationCode
            {
                Email = email,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            });

        await _unitOfWork.SaveChangesAsync();

        //   send Email
        await _emailService.SendEmailAsync(
            email,
            "Verify Your Email - TechWise",
            $"""
         <h2>Email Verification</h2>
         <p>Your verification code is:</p>
         <h1 style="color:#2563eb">{code}</h1>
         <p>This code expires in 10 minutes.</p>
         """
        );
    }

    public async Task<UserResponse> VerifyEmailAsync(VerifyEmailRequest request)
    {
        var code = await _unitOfWork.EmailVerificationCodes
            .GetValidCodeAsync(request.Email, request.Code);

        if (code is null || code.IsExpired)
            throw new BadRequestException("Invalid or expired verification code");

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) throw new UserNotFoundException(request.Email);

        user.EmailConfirmed = true;
        code.IsUsed = true;

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserResponse
        {
            useId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Token = await GenerateTokenAsync(user)
        };
    }



    private async Task<string> GenerateTokenAsync(AppUser user)
    {
        var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var jwtOptions = _jwtOptions.Value;
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: authClaims,
            expires: DateTime.UtcNow.AddDays(jwtOptions.DurationDays),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}