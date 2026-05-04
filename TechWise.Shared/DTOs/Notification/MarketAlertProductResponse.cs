namespace TechWise.Shared.DTOs.Notification
{
    public class MarketAlertProductResponse
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

}
