namespace TechWise.Shared.DTOs.Products
{
    public class SavedItemResponse
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime SavedAt { get; set; }
        public double? RatingAvg { get; set; }
        public Dictionary<string, string> KeySpecs { get; set; } = new();
    }
}