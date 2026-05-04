namespace TechWise.Shared.DTOs.Cart
{
    public class CartResponse
    {
        public int Id { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}