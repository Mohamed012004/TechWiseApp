using Microsoft.AspNetCore.Identity;
using TechWise.Domains.Entities.Identity;
using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Files;
using TechWise.Services.Abstractions.Profile;
using TechWise.Shared.DTOs.Profile;

namespace TechWise.Services.Profile
{
    public class ProfileService(
        UserManager<AppUser> _userManager,
        IFileService _fileService
    ) : IProfileService
    {
        public async Task<GetProfileResponse> GetProfileAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);

            return MapToResponse(user);
        }

        public async Task<GetProfileResponse> UpdateProfileAsync(
            string email, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Location = request.Location;

            await _userManager.UpdateAsync(user);

            return MapToResponse(user);
        }

        public async Task<GetProfileResponse> UpdateProfilePhotoAsync(
            string email, Stream imageStream, string fileName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);

            // Upload Photo on Cloudinary
            var photoUrl = await _fileService.UploadImageAsync(imageStream, fileName);

            user.ProfilePhoto = photoUrl;
            await _userManager.UpdateAsync(user);

            return MapToResponse(user);
        }

        public async Task DeleteAccountAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);
            if (user.IsDeleted) throw new AccountDeletedException();

            user.OriginalEmail = email;
            user.Email = $"deleted_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{user.Email}";
            user.UserName = user.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            user.NormalizedUserName = user.Email.ToUpper();

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.UpdateAsync(user);
        }



        private static GetProfileResponse MapToResponse(AppUser user)
        {
            return new GetProfileResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                ProfilePhoto = user.ProfilePhoto,
                IsVerified = user.EmailConfirmed,
                MemberSince = user.CreatedAt.ToString("MMM yyyy")
            };
        }

    }
}