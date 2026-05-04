// SuperAdminService.cs
using Microsoft.AspNetCore.Identity;
using TechWise.Domains.Entities.Identity;
using TechWise.Domains.Entities.SuperAdmin;
using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Services.Abstractions.Admin;
using TechWise.Shared.DTOs.Admin;

namespace TechWise.Services.Admin
{
    public class SuperAdminService(
        UserManager<AppUser> _userManager
    ) : ISuperAdminService
    {
        public async Task<List<AdminUserResponse>> GetAdminsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            return admins
                .Where(a => !a.IsDeleted)
                .Select(a => new AdminUserResponse
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email!,
                    CreatedAt = a.CreatedAt,
                    Role = "Admin"
                }).ToList();
        }

        public async Task<AdminUserResponse> CreateAdminAsync(
            CreateAdminRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                throw new BadRequestException("Email already exists");

            var admin = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(admin, request.Password);
            if (!result.Succeeded)
                throw new BadRequestException(
                    result.Errors.First().Description);

            await _userManager.AddToRoleAsync(admin, "Admin");

            return new AdminUserResponse
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email!,
                CreatedAt = admin.CreatedAt,
                Role = "Admin"
            };
        }

        public async Task DeleteAdminAsync(string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin is null) throw new UserNotFoundException(adminId);

            var roles = await _userManager.GetRolesAsync(admin);
            if (!roles.Contains("Admin"))
                throw new BadRequestException("User is not an Admin");

            admin.IsDeleted = true;
            admin.DeletedAt = DateTime.UtcNow;
            admin.OriginalEmail = admin.Email;
            admin.Email = $"deleted_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{admin.Email}";
            admin.UserName = admin.Email;
            admin.NormalizedEmail = admin.Email!.ToUpper();
            admin.NormalizedUserName = admin.Email.ToUpper();

            await _userManager.UpdateSecurityStampAsync(admin);
            await _userManager.UpdateAsync(admin);
        }
    }
}