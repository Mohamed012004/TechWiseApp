namespace TechWise.Shared.DTOs.Orders
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }

        // Shipping
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        // stripe payment details
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public bool IsPaid { get; set; } = false;

        public List<OrderItemResponse> Items { get; set; } = new();
    }
}