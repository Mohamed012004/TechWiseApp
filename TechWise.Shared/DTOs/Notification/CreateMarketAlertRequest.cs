namespace TechWise.Shared.DTOs.Notifications
{
    public class CreateMarketAlertRequest
    {
        public string Title { get; set; }
        public List<int> ProductIds { get; set; } = new();
    }
}