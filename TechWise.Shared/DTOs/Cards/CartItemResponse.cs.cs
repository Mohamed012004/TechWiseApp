namespace TechWise.Shared.DTOs.Cart
{
    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal => Price * Quantity;
    }
}