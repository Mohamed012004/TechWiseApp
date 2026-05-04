// ISuperAdminService.cs
using TechWise.Domains.Entities.SuperAdmin;
using TechWise.Shared.DTOs.Admin;

namespace TechWise.Services.Abstractions.Admin
{
    public interface ISuperAdminService
    {
        Task<List<AdminUserResponse>> GetAdminsAsync();
        Task<AdminUserResponse> CreateAdminAsync(CreateAdminRequest request);
        Task DeleteAdminAsync(string adminId);
    }
}