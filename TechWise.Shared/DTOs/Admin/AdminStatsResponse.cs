namespace TechWise.Shared.DTOs.Admin
{
    public class AdminStatsResponse
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalReviews { get; set; }
    }
}