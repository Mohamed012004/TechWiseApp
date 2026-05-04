using Microsoft.AspNetCore.Identity;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;

namespace TechWise.Persistence
{
    public class DbIntializer
        (
           UserManager<AppUser> _userManager,
           RoleManager<IdentityRole> _roleManager
        ) : IDbIntializer
    {
        public async Task InitializeIdentityAsync()
        {
            // Create role if Not Found
            await EnsureRoleAsync("SuperAdmin");
            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("User");

            // create SuperAdmin Only one time
            var superAdminEmail = "SuperAdmin@gmail.com";
            var existingSuperAdmin = await _userManager.FindByEmailAsync(superAdminEmail);

            if (existingSuperAdmin == null)
            {
                var superAdmin = new AppUser
                {
                    UserName = "SuperAdmin",
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(superAdmin, "P@ssW0rd");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
            }
            else
            {
                var currentRoles = await _userManager.GetRolesAsync(existingSuperAdmin);
                if (!currentRoles.Contains("SuperAdmin"))
                {
                    await _userManager.AddToRoleAsync(existingSuperAdmin, "SuperAdmin");
                }
            }

            // create Admin Only one time
            var adminEmail = "Admin@gmail.com";
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var admin = new AppUser
                {
                    UserName = "Admin",
                    FirstName = "Admin",
                    LastName = "",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "P@ssW0rd");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                var currentRoles = await _userManager.GetRolesAsync(existingAdmin);
                if (!currentRoles.Contains("Admin"))
                {
                    await _userManager.AddToRoleAsync(existingAdmin, "Admin");
                }
            }
        }

        // if the role does not exist, create it
        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}