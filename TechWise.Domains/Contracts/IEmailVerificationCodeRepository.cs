using TechWise.Domains.Entities.Identity;

namespace TechWise.Domains.Contracts
{
    public interface IEmailVerificationCodeRepository
    {
        Task AddAsync(EmailVerificationCode code);
        Task DeleteOldCodesAsync(string email);
        Task<EmailVerificationCode?> GetValidCodeAsync(string email, string code);
    }
}