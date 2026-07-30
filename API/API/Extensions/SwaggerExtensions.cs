using API.Swagger.Options;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var swaggerOptions = configuration
                .GetSection(SwaggerOptions.SectionName)
                .Get<SwaggerOptions>() ?? new SwaggerOptions();

            services.Configure<SwaggerOptions>(
                configuration.GetSection(SwaggerOptions.SectionName));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(swaggerOptions.DocumentName, new OpenApiInfo
                {
                    Title = swaggerOptions.Title,
                    Version = swaggerOptions.Version,
                    Description = swaggerOptions.Description
                });

                options.AddSecurityDefinition(swaggerOptions.SecuritySchemeName, new OpenApiSecurityScheme
                {
                    Name = swaggerOptions.HeaderName,
                    Description = swaggerOptions.SecurityDescription,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = swaggerOptions.Scheme,
                    BearerFormat = swaggerOptions.BearerFormat
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = swaggerOptions.SecuritySchemeName
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            var swaggerOptions = app.ApplicationServices
                .GetRequiredService<IConfiguration>()
                .GetSection(SwaggerOptions.SectionName)
                .Get<SwaggerOptions>() ?? new SwaggerOptions();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = swaggerOptions.Title;
                options.SwaggerEndpoint(
                    $"/swagger/{swaggerOptions.DocumentName}/swagger.json",
                    $"{swaggerOptions.Title} {swaggerOptions.Version}");
            });

            return app;
        }
    }
}
