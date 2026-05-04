using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Identity;
using TechWise.Persistence;
using TechWise.Persistence.Identity.Contexts;
using TechWise.Persistence.Store;
using TechWise.Services;
using TechWise.Shared;
using TechWise.Web.Middelwares;
using TechWiseApp.Web.Hubs;

namespace TechWise.Web.Extentions
{
    public static class Extentions
    {
        public static IServiceCollection AddAllServices(
            this IServiceCollection service, IConfiguration configuration)
        {
            service.AddWebServices();
            service.AddIdentityServices(configuration);
            service.AddInfrastructureServices(configuration);
            service.AddApplicationServices(configuration);
            service.AddSignalR();
            service.Configure<JWTOptions>(configuration.GetSection("JWTOptions"));

            service.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return service;
        }

        private static IServiceCollection AddWebServices(this IServiceCollection service)
        {
            service.AddControllers();
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return service;
        }

        private static IServiceCollection AddIdentityServices(
            this IServiceCollection service, IConfiguration configuration)
        {
            service.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 7;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            })
            .AddEntityFrameworkStores<IdentityStoreDbContext>()
            .AddDefaultTokenProviders();

            var jwtOptions = configuration.GetSection("JWTOptions").Get<JWTOptions>();

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // To make SignlR Operate with JWT
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions!.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                };
            });

            return service;
        }

        public static async Task<WebApplication> ConfigureMiddelware(this WebApplication app)
        {
            //await app.SeedData();

            // strip webook need row body
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api/payment/stripe/webhook",
                    StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.EnableBuffering();
                }
                await next();
            });

            app.UseGlobalErroHandleng();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");

            return app;
        }

        private static async Task<WebApplication> SeedData(this WebApplication app)
        {
            var maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using var scope = app.Services.CreateScope();

                    // Identity Seeding
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
                    await dbInitializer.InitializeIdentityAsync();

                    // Store Seeding
                    var storeContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
                    await StoreDbInitializer.SeedAsync(storeContext);

                    break; // if succeeded - break
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    // wait 5 minutes and try again
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("SeedData");
                    logger.LogError(ex, "SeedData failed after {MaxRetries} attempts. App will continue.", maxRetries);
                }
            }

            return app;
        }



        private static WebApplication UseGlobalErroHandleng(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddelware>();
            return app;
        }
    }
}