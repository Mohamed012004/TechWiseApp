using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;
using TechWise.Persistence.Identity.Contexts;

namespace TechWise.Persistence.Identity.Repositories
{
    public class EmailVerificationCodeRepository(IdentityStoreDbContext _context)
        : IEmailVerificationCodeRepository
    {
        public async Task AddAsync(EmailVerificationCode code)
        {
            await _context.EmailVerificationCodes.AddAsync(code);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOldCodesAsync(string email)
        {
            var oldCodes = await _context.EmailVerificationCodes
                .Where(c => c.Email == email)
                .ToListAsync();

            _context.EmailVerificationCodes.RemoveRange(oldCodes);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailVerificationCode?> GetValidCodeAsync(
            string email, string code)
        {
            return await _context.EmailVerificationCodes
                .FirstOrDefaultAsync(c =>
                    c.Email == email &&
                    c.Code == code &&
                    !c.IsUsed &&
                    c.ExpiresAt > DateTime.UtcNow);
        }
    }
}