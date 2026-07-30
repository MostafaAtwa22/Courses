using Domain.Entities.Identity;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Seed;
using Infrastructure.Persistence.Seed.Identity;
using Microsoft.AspNetCore.Identity;
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
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await ApplicationIdentityDbContextSeed.SeedAsync(context, userManager, roleManager, loggerFactory);
            await ApplicationDbContextSeed.SeedAsync(context, loggerFactory);

            return app;
        }
    }
}