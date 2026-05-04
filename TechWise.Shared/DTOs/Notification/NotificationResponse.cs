namespace TechWise.Shared.DTOs.Notifications
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public int? ProductId { get; set; }
        public int? OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}