using System.Text.Json;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed
{
    public static class ApplicationDbContextSeed
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static async Task SeedAsync(ApplicationDbContext context, 
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();

            if (!context.Categories.Any())
                await SeedCategoriesAsync(context, logger);
                
            if (!context.Courses.Any())
                await SeedCoursesAsync(context, logger);

            if (!context.Reviews.Any())
                await SeedReviewsAsync(context, logger);

            if (!context.Enrollments.Any())
                await SeedEnrollmentsAsync(context, logger);
            
            logger.LogInformation("Database seeding completed.");
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            var path = GetSeedPath("Categories.json");
            var categoryData = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(categoryData);
            var categoriesElement = document.RootElement.GetProperty("categories");
            var categories = categoriesElement.Deserialize<List<Category>>(_options) ?? [];

            foreach (var category in categories)
                context.Categories.Add(category);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded categories data.");
        }


        private static async Task SeedCoursesAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Courses.json");
            var courseData = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(courseData);
            var coursesElement = document.RootElement.GetProperty("courses");
            var courses = coursesElement.Deserialize<List<Course>>(_options) ?? [];

            foreach (var course in courses)
                context.Courses.Add(course);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded courses data.");
        }

        private static async Task SeedReviewsAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Reviews.json");
            var reviewData = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(reviewData);
            var reviewsElement = document.RootElement.GetProperty("reviews");
            var reviews = reviewsElement.Deserialize<List<Review>>(_options) ?? [];

            foreach (var review in reviews)
                context.Reviews.Add(review);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded reviews data.");
        }


        private static async Task SeedEnrollmentsAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Enrollments.json");
            var data = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(data);
            var element = document.RootElement.GetProperty("enrollments");
            var enrollments = element.Deserialize<List<Enrollment>>(_options) ?? [];

            foreach (var enrollment in enrollments)
                context.Enrollments.Add(enrollment);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded enrollments data.");
        }

        private static string GetSeedPath (string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, "Persistence", "Seed", "Files", fileName);
        }
    }
}