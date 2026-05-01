using System.Text.Json;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed.Identity
{
    public static class ApplicationIdentityDbContextSeed
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static async Task SeedAsync(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();

            if (!context.Roles.Any())
                await SeedRolesAsync(roleManager, logger);

            if (!context.Users.Any())
                await SeedUsersAndRolesAsync(userManager, logger);

            if (!context.Instructors.Any())
                await SeedInstructorsAsync(context, logger);

            if (!context.Students.Any())
                await SeedStudentsAsync(context, logger);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            var path = GetSeedPath("Roles.json");
            var data = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(data);
            var rolesElement = document.RootElement.GetProperty("roles");
            var roles = rolesElement.Deserialize<List<IdentityRole>>(_options) ?? [];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    await roleManager.CreateAsync(role);
                }
            }
            logger.LogInformation("Seeded identity roles.");
        }

        private static async Task SeedUsersAndRolesAsync(
            UserManager<ApplicationUser> userManager, 
            ILogger logger)
        {
            var path = GetSeedPath("Users.json");
            var data = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(data);
            var usersElement = document.RootElement.GetProperty("users");

            foreach (var userElement in usersElement.EnumerateArray())
            {
                var user = userElement.Deserialize<ApplicationUser>(_options);
                if (user == null) continue;

                if (userElement.TryGetProperty("id", out var idProp))
                    user.Id = idProp.GetString()!;

                user.EmailConfirmed = true;
                user.NormalizedEmail = user.Email?.ToUpper();
                user.NormalizedUserName = user.UserName?.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();
                
                var result = await userManager.CreateAsync(user, "P@ssw0rd123!");
                if (result.Succeeded)
                {
                    if (userElement.TryGetProperty("role", out var roleProp))
                    {
                        var roleName = roleProp.GetString();
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            await userManager.AddToRoleAsync(user, roleName);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                        logger.LogError($"❌ Error seeding user {user.UserName}: {error.Description}");
                }
            }
            logger.LogInformation("Seeded users and assigned roles.");
        }

        private static async Task SeedInstructorsAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Instructors.json");
            var instructorData = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(instructorData);
            var instructorsElement = document.RootElement.GetProperty("instructors");
            var instructors = instructorsElement.Deserialize<List<Instructor>>(_options) ?? [];

            foreach (var instructor in instructors)
                context.Instructors.Add(instructor);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded instructors data.");
        }

        private static async Task SeedStudentsAsync(ApplicationDbContext context, ILogger logger)
        {
            var path = GetSeedPath("Students.json");
            var studentData = await File.ReadAllTextAsync(path);
            
            using var document = JsonDocument.Parse(studentData);
            var studentsElement = document.RootElement.GetProperty("students");
            var students = studentsElement.Deserialize<List<Student>>(_options) ?? [];

            foreach (var student in students)
                context.Students.Add(student);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded students data.");
        }

        private static string GetSeedPath(string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, "Persistence", "Seed", "Files", fileName);
        }
    }
}
