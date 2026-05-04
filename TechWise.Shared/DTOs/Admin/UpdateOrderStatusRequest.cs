using TechWise.Shared.Enums;

namespace TechWise.Shared.DTOs.Admin
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}