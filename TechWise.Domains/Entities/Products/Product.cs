 
namespace TechWise.Domains.Entities.Store
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal? WasPrice { get; set; }
        public int InStock { get; set; }
        public double? RatingAvg { get; set; }
        public int? RatingCount { get; set; }
        public string? ImageUrl { get; set; }
        public string? ProductUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ICollection<ProductSpec> Specs { get; set; } = new List<ProductSpec>();
    }
}