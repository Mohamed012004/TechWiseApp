using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;
using TechWise.Services.Abstractions;
using TechWise.Services.Abstractions.Admin;
using TechWise.Services.Abstractions.Cart;
using TechWise.Services.Abstractions.Email;
using TechWise.Services.Abstractions.Files;
using TechWise.Services.Abstractions.Notifications;
using TechWise.Services.Abstractions.Orders;
using TechWise.Services.Abstractions.Products;
using TechWise.Services.Abstractions.Profile;
using TechWise.Services.Abstractions.Reviews;
using TechWise.Services.Abstractions.Support;
using TechWise.Services.Admin;
using TechWise.Services.Carts;
using TechWise.Services.Email;
using TechWise.Services.Files;
using TechWise.Services.Notifications;
using TechWise.Services.Orders;
using TechWise.Services.Products;
using TechWise.Services.Profile;
using TechWise.Services.Reviews;
using TechWise.Services.Support;
using TechWise.Shared;
using TechWise.Shared.DTOs.Settings;
using TechWise.Shared.Settings;
using TechWiseApp.Web.Hubs;

namespace TechWise.Services
{
    public class ServiceManager(
        UserManager<AppUser> _userManager,
        IOptions<JWTOptions> _jwtOptions,
        IOptions<EmailSettings> _emailSettings,
        IOptions<CloudinarySettings> _cloudinarySettings,
        IOptions<StripeSettings> _stripeSettings,
        IUnitOfWork _unitOfWork,
        IHttpClientFactory _httpClientFactory,
        IHubContext<NotificationHub> _hubContext
    ) : IServiceManager
    {
        private IAuthService? _authService;
        private IEmailService? _emailService;
        private IFileService? _fileService;
        private IProfileService? _profileService;
        private IProductService? _productService;
        private ICartService? _cartService;
        private IOrderService? _orderService;
        private IReviewService? _reviewService;
        private IAdminService? _adminService;
        private ISuperAdminService? _superAdminService;
        private ISupportService? _supportService;
        private INotificationService? _notificationService;

        public IEmailService EmailService =>
            _emailService ??= new EmailService(_emailSettings);

        public IFileService FileService =>
            _fileService ??= new FileService(_cloudinarySettings);

        public IAuthService AuthService =>
             _authService ??= new AuthService(
                 _userManager, _jwtOptions, EmailService, _unitOfWork, _httpClientFactory);

        public IProfileService ProfileService =>
            _profileService ??= new ProfileService(_userManager, FileService);

        public IProductService ProductService =>
         _productService ??= new ProductService(_unitOfWork);

        public ICartService CartService =>
             _cartService ??= new CartService(_unitOfWork);

        public IOrderService OrderService =>
            _orderService ??= new OrderService(_unitOfWork, _stripeSettings, NotificationService);

        public IReviewService ReviewService =>
            _reviewService ??= new ReviewService(_unitOfWork);

        public IAdminService AdminService =>
            _adminService ??= new AdminService(_unitOfWork, _userManager, NotificationService);

        public ISuperAdminService SuperAdminService =>
            _superAdminService ??= new SuperAdminService(_userManager);

        public ISupportService SupportService =>
            _supportService ??= new SupportService(_unitOfWork, EmailService);

        public INotificationService NotificationService =>
            _notificationService ??= new NotificationService(_unitOfWork, _hubContext);
    }
}
