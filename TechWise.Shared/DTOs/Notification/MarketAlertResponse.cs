namespace TechWise.Shared.DTOs.Notification
{
    public class MarketAlertResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MarketAlertProductResponse> Products { get; set; } = new();
    }
}
