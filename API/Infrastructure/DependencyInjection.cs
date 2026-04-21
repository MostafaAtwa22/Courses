using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Data;
using Application.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Infrastructure.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Default connection string is not configured");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        services.AddHealthChecks()
            .AddNpgSql(connectionString, 
                name: "PostgreSQL",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "postgresql" })
            .AddRedis(connectionString,
                name: "Redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "redis" });
        return services;
    }
}
