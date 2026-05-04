namespace TechWise.Shared.DTOs.Notifications
{
    public class NotificationSettingsResponse
    {
        public bool PushNotifications { get; set; }
        public bool EmailUpdates { get; set; }
        public bool PriceDropAlerts { get; set; }
    }
}