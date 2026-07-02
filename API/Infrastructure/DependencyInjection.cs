using Microsoft.Extensions.Configuration;
using Hangfire;
using Hangfire.Redis.StackExchange;
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
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Identity;
using Infrastructure.Identity.Authentication.Facebook;
using Infrastructure.Identity.Authentication.Google;
using constant = Domain.Constants;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString(constant.IdentityConstants.DefaultConnection) 
            ?? throw new InvalidOperationException("Default connection string is not configured");

        services
            .AddPersistence(connectionString)
            .AddIdentityServices(config)
            .AddCaching(config)
            .AddInfrastructureServices()
            .AddBackgroundJobs(config)
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
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
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
            options.Tokens.EmailConfirmationTokenProvider = constant.IdentityConstants.EmailOtpProvider;
            options.Tokens.ProviderMap[constant.IdentityConstants.EmailOtpProvider] = new TokenProviderDescriptor(
                typeof(EmailTokenProvider<ApplicationUser>)
            );
            // user settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = constant.IdentityConstants.AllowedUserNameCharacters;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailTokenProvider<ApplicationUser>>(constant.IdentityConstants.EmailOtpProvider);

        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<IIdentityEmailService, IdentityEmailService>();
        services.AddTransient<ITwoFactorService, TwoFactorService>();

        services.Configure<GoogleOptions>(config.GetSection(GoogleOptions.SectionName));
        services.Configure<FacebookOptions>(config.GetSection(FacebookOptions.SectionName));
        services.AddHttpClient();
        
        services.AddScoped<IExternalLoginValidator, GoogleValidator>();
        services.AddScoped<IExternalLoginValidator, FacebookValidator>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IConnectionMultiplexer>(c => 
        {
            var redis = config.GetConnectionString(constant.IdentityConstants.Cache) 
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
        services.AddTransient<EmailService>();
        services.AddTransient<IEmailService, BackgroundEmailService>();
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
                tags: new[] { "db", "redis" })
            .AddHangfire(options => {
                options.MinimumAvailableServers = 1;
            });
        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration config)
    {
        var redisConnection = config.GetConnectionString(constant.IdentityConstants.Cache) 
            ?? throw new InvalidOperationException("Redis connection string is not configured");

        services.AddHangfire(configuration =>
        {
            configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redisConnection, new RedisStorageOptions
                {
                    Prefix = "hangfire:",          // namespace to avoid key collisions with cache
                    Db = 1,                        // use DB 1 for Hangfire, DB 0 for cache
                    SucceededListSize = 500,       // keep last 500 succeeded jobs
                    DeletedListSize = 200,
                });
        });

        services.AddHangfireServer(options =>
        {
            options.Queues = new[] { "emails", "default" };
            options.WorkerCount = Environment.ProcessorCount * 2;
        });

        return services;
    }
}
