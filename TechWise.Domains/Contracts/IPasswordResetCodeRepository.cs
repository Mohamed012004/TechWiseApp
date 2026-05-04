using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;

public interface IPasswordResetCodeRepository
    : IGenericRepository<PasswordResetCode>
{
    Task DeleteOldCodesAsync(string email);
    Task<PasswordResetCode?> GetValidCodeAsync(string code);

    Task<PasswordResetCode?> GetByResetTokenAsync(string resetToken);
}