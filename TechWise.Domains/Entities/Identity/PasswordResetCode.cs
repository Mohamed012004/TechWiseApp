namespace TechWise.Domains.Entities.Identity
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string? ResetToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}