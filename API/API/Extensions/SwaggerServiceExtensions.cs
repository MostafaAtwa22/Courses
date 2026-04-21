using Microsoft.OpenApi;

namespace API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerervices(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Course Project API",
                    Description = "Our project endpoints",
                    Contact = new OpenApiContact
                    {
                        Name = "Courses",
                        Email = "atwamostafa5@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Course Project"
                    }
                });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the input field.\r\nExample: \"Bearer abcdef12345\"",
                });

                option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });
            return services;
        }
    }
}