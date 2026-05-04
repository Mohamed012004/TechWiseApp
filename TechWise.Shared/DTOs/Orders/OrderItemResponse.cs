namespace TechWise.Shared.DTOs.Orders
{
    public class OrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal => Price * Quantity;
    }
}