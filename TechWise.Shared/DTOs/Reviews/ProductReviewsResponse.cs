namespace TechWise.Shared.DTOs.Reviews
{
    public class ProductReviewsResponse
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = new();
    }
}