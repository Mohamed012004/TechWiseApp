using TechWise.Shared.Enums;

namespace TechWise.Shared.DTOs.Admin
{
    public class AdminOrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ItemsCount { get; set; }
    }
}