using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Entities.Identity;
using TechWise.Persistence.Identity.Contexts;
using TechWise.Persistence.Identity.Repositories;

public class PasswordResetCodeRepository(IdentityStoreDbContext _context)
    : GenericRepository<PasswordResetCode>(_context),
      IPasswordResetCodeRepository
{
    public async Task DeleteOldCodesAsync(string email)
    {
        var oldCodes = await _context.PasswordResetCodes
            .Where(c => c.Email == email)
            .ToListAsync();

        if (oldCodes.Any())
            _context.PasswordResetCodes.RemoveRange(oldCodes);
    }

    public async Task<PasswordResetCode?> GetValidCodeAsync(string code)
    {
        return await _context.PasswordResetCodes
            .Where(c => c.Code == code && !c.IsUsed)
            .OrderByDescending(c => c.ExpiresAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PasswordResetCode?> GetByResetTokenAsync(string resetToken)
    {
        return await _context.PasswordResetCodes
            .Where(c => c.ResetToken == resetToken && !c.IsUsed)
            .FirstOrDefaultAsync();
    }
}