namespace TechWise.Shared.DTOs.Notification
{
    public class UpdateNotificationSettingsRequest
    {
        public bool PushNotifications { get; set; }
        public bool EmailUpdates { get; set; }
        public bool PriceDropAlerts { get; set; }
    }
}