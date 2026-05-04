
using TechWise.Shared.DTOs.Profile;

namespace TechWise.Services.Abstractions.Profile
{
    public interface IProfileService
    {
        Task<GetProfileResponse> GetProfileAsync(string email);
        Task<GetProfileResponse> UpdateProfileAsync(string email, UpdateProfileRequest request);
        Task<GetProfileResponse> UpdateProfilePhotoAsync(string email, Stream imageStream, string fileName);
        Task DeleteAccountAsync(string email);

    }
}