using TechWise.Domains.Contracts;
using TechWise.Persistence.Identity.Contexts;
using TechWise.Persistence.Identity.Repositories;
using TechWise.Persistence.Store;
using TechWise.Persistence.Store.Repositories;

namespace TechWise.Persistence.Identity
{
    public class UnitOfWork(IdentityStoreDbContext _context, StoreDbContext _storeContext) : IUnitOfWork
    {
        private IPasswordResetCodeRepository? _passwordResetCodes;
        private IEmailVerificationCodeRepository? _emailVerificationCodes;

        private IProductRepository? _products;
        private IRecommendedProductRepository? _recommendedProducts;
        private ISavedItemRepository? _savedItems;
        private ICartRepository? _cart;
        private IOrderRepository? _orders;
        private IReviewRepository? _reviews;
        private IAdminRepository? _admin;
        private ISupportRepository? _support;
        private INotificationRepository? _notifications;
        private INotificationSettingsRepository? _notificationSettings;
        private IMarketAlertRepository? _marketAlerts;


        public IPasswordResetCodeRepository PasswordResetCodes =>
            _passwordResetCodes ??= new PasswordResetCodeRepository(_context);

        public IEmailVerificationCodeRepository EmailVerificationCodes =>
            _emailVerificationCodes ??= new EmailVerificationCodeRepository(_context);


        public IProductRepository Products =>
               _products ??= new ProductRepository(_storeContext);

        public IRecommendedProductRepository RecommendedProducts =>
        _recommendedProducts ??= new RecommendedProductRepository(_storeContext);


        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();


        public ISavedItemRepository SavedItems =>
        _savedItems ??= new SavedItemRepository(_storeContext);


        public ICartRepository Cart =>
         _cart ??= new CartRepository(_storeContext);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_storeContext);

        public IReviewRepository Reviews =>
         _reviews ??= new ReviewRepository(_storeContext);


        public IAdminRepository Admin =>
         _admin ??= new AdminRepository(_storeContext);

        public ISupportRepository Support =>
            _support ??= new SupportRepository(_storeContext);


        public INotificationRepository Notifications =>
            _notifications ??= new NotificationRepository(_storeContext);

        public INotificationSettingsRepository NotificationSettings =>
            _notificationSettings ??= new NotificationSettingsRepository(_storeContext);

        public IMarketAlertRepository MarketAlerts =>
            _marketAlerts ??= new MarketAlertRepository(_storeContext);

    }
}