namespace TechWise.Shared.DTOs.Admin
{
    public class UpdateProductRequest
    {
        public decimal Price { get; set; }
        public decimal? WasPrice { get; set; }
        public int InStock { get; set; }
    }
}