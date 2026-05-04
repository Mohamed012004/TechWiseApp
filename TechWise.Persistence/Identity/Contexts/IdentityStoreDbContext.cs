using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Entities.Identity;

namespace TechWise.Persistence.Identity.Contexts
{
    public class IdentityStoreDbContext(DbContextOptions<IdentityStoreDbContext> options) : IdentityDbContext<AppUser>(options)
    {

        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<Address>().ToTable("Addresses");

            builder.Entity<PasswordResetCode>().ToTable("PasswordResetCodes");

            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();


        }
    }
}
