using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> UseAutoMigrationAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            await ApplicationDbContextSeed.SeedAsync(context, loggerFactory);
            
            return app;
        }
    }
}