using System.Text.Json;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, 
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();

            if (!context.Categories.Any())
                await SeedCategoriesAsync(context, logger);
                
            if (!context.Courses.Any())
                await SeedCoursesAsync(context, logger);
            
            logger.LogInformation("Database seeding completed.");
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            var path = GetSeedPath("Categories.json");
            var categoryData = await File.ReadAllTextAsync(path);
            var categories = JsonSerializer.Deserialize<List<Category>>(categoryData)
                ?? [];

            foreach (var category in categories)
                context.Categories.Add(category);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded categories data.");
        }

        private static async Task SeedCoursesAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Courses.json");
            var courseData = await File.ReadAllTextAsync(path);
            var courses = JsonSerializer.Deserialize<List<Course>>(courseData)
                ?? [];

            foreach (var course in courses)
                context.Courses.Add(course);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded courses data.");
        }

        private static string GetSeedPath (string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, "Seed", "Files", fileName);
        }
    }
}