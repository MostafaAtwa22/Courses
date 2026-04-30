using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Infrastructure.Repositories;
using Infrastructure.Services;
using StackExchange.Redis;
using Infrastructure.Cache;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connectionString = config.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Default connection string is not configured");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        services.AddSingleton<IConnectionMultiplexer>(c => 
        {
            var redis = config.GetConnectionString("Cache") 
                ?? throw new InvalidOperationException("Redis connection string is not configured");

            var configuration = ConfigurationOptions.Parse(redis, true);

            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IFileService, FileService>();
        
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
