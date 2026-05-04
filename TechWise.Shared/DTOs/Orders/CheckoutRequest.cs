using TechWise.Shared.Enums;

namespace TechWise.Shared.DTOs.Orders
{
    public class CheckoutRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PromoCode { get; set; }
    }
}