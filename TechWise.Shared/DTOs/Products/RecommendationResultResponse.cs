namespace TechWise.Shared.DTOs.Products
{
    public class RecommendationResultResponse
    {
        public int ProductId { get; set; }
        public int UserReqNumber { get; set; }
        public string? OperationType { get; set; }
        public string? Purpose { get; set; }
        public string? Budget { get; set; }
        public string? Reason { get; set; }

        // Product Info
        public string Title { get; set; }
        public string Brand { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? WasPrice { get; set; }
        public double? RatingAvg { get; set; }
        public Dictionary<string, string> KeySpecs { get; set; } = new();
    }
}