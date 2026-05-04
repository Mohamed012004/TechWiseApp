using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWise.Domains.Contracts;
using TechWise.Persistence.Identity;
using TechWise.Persistence.Identity.Contexts;
using TechWise.Persistence.Store;

namespace TechWise.Persistence
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityStoreDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("IdentityConnection"),
                    npgsql =>
                    {
                        npgsql.CommandTimeout(60);
                        npgsql.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });
            });

            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("StoreConnection"),
                    npgsql =>
                    {
                        npgsql.CommandTimeout(60);
                        npgsql.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });
            });

            services.AddScoped<IDbIntializer, DbIntializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}