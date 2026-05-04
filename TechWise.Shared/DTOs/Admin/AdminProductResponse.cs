namespace TechWise.Shared.DTOs.Admin
{
    public class AdminProductResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public double? RatingAvg { get; set; }
        public string? ImageUrl { get; set; }
    }
}