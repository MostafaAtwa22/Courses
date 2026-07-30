using API.Filters;
using Application.Common.Interfaces;
using Domain.Entities.Identity;
using Hangfire;
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

        public static IApplicationBuilder UseScheduledBackgroundJobs(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthFilter() },
                DashboardTitle = "EduFocus - Background Jobs"
            });

            RecurringJob.AddOrUpdate<IDiscountJobService>(
                "deactivate-expired-discounts",
                discountJobService => discountJobService.DeactivateExpiredDiscountsAsync(),
                Cron.Hourly);

            return app;
        }
    }
}
