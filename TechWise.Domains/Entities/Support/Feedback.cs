namespace TechWise.Domains.Entities.Support
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}