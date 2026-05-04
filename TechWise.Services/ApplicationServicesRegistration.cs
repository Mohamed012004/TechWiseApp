using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWise.Services.Abstractions;
using TechWise.Shared.DTOs.Settings;
using TechWise.Shared.Settings;

namespace TechWise.Services
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpClient();
            services.AddScoped<IServiceManager, ServiceManager>();  // Allow Dependency Injection for ServiceManger

            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));

            services.Configure<CloudinarySettings>(
                 configuration.GetSection("CloudinarySettings"));

            services.Configure<StripeSettings>(
                configuration.GetSection("StripeSettings"));


            //services.AddAutoMapper(M => M.AddProfile(new AuthProfile()));

            return services;
        }
    }
}
