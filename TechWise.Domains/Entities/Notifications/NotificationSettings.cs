namespace TechWise.Domains.Entities.Store
{
    public class NotificationSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool PushNotifications { get; set; } = true;
        public bool EmailUpdates { get; set; } = true;
        public bool PriceDropAlerts { get; set; } = true;
    }
}