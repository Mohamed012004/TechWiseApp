
namespace TechWise.Domains.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IPasswordResetCodeRepository PasswordResetCodes { get; }

        IEmailVerificationCodeRepository EmailVerificationCodes { get; }

        IProductRepository Products { get; }
        IRecommendedProductRepository RecommendedProducts { get; }
        ISavedItemRepository SavedItems { get; }

        ICartRepository Cart { get; }
        IOrderRepository Orders { get; }
        IReviewRepository Reviews { get; }
        IAdminRepository Admin { get; }
        ISupportRepository Support { get; }


        INotificationRepository Notifications { get; }
        INotificationSettingsRepository NotificationSettings { get; }
        IMarketAlertRepository MarketAlerts { get; }

        Task<int> SaveChangesAsync();
    }
}