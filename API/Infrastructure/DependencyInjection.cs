using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Infrastructure.Repositories;
using Infrastructure.Identity;
using Infrastructure.Services;
using StackExchange.Redis;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Email;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Identity;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Default connection string is not configured");

        services
            .AddPersistence(connectionString)
            .AddIdentityServices()
            .AddCaching(config)
            .AddInfrastructureServices()
            .AddHealthCheckServices(connectionString, config);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            
            // Confirm email
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            
            // user settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<IIdentityEmailService, IdentityEmailService>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IConnectionMultiplexer>(c => 
        {
            var redis = config.GetConnectionString("Cache") 
                ?? throw new InvalidOperationException("Redis connection string is not configured");

            var configuration = ConfigurationOptions.Parse(redis, true);
            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
        
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<IUrlProvider, UrlProvider>();
        services.AddHttpContextAccessor();
        
        return services;
    }

    private static IServiceCollection AddHealthCheckServices(this IServiceCollection services, string connectionString, IConfiguration config)
    {
        services.AddHealthChecks()
            .AddNpgSql(connectionString, 
                name: "PostgreSQL",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "postgresql" })
            .AddRedis(config.GetConnectionString("Cache") ?? "",
                name: "Redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "redis" });
                
        return services;
    }
}
