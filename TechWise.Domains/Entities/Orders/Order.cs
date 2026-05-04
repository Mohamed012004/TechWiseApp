using TechWise.Shared.Enums;

namespace TechWise.Domains.Entities.Store
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Confirmed;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Shipping Address
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        // Stripe Payment Details
        public string? PaymentIntentId { get; set; }
        public bool IsPaid { get; set; } = false;


        // Navigation
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}