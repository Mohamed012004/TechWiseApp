using TechWise.Shared.Enums;

namespace TechWise.Domains.Entities.Store
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product? Product { get; set; }

    }
}