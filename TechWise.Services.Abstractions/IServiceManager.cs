
using TechWise.Services.Abstractions.Admin;
using TechWise.Services.Abstractions.Cart;
using TechWise.Services.Abstractions.Email;
using TechWise.Services.Abstractions.Notifications;
using TechWise.Services.Abstractions.Orders;
using TechWise.Services.Abstractions.Products;
using TechWise.Services.Abstractions.Profile;
using TechWise.Services.Abstractions.Reviews;
using TechWise.Services.Abstractions.Support;

namespace TechWise.Services.Abstractions
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }

        IEmailService EmailService { get; }

        IProfileService ProfileService { get; }

        IProductService ProductService { get; }

        ICartService CartService { get; }
        IOrderService OrderService { get; }
        IReviewService ReviewService { get; }
        IAdminService AdminService { get; }
        ISuperAdminService SuperAdminService { get; }

        ISupportService SupportService { get; }
        INotificationService NotificationService { get; }


    }
}
