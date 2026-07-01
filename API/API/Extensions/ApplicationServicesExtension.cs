using API.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<UrlsOptions>(config.GetSection(UrlsOptions.SectionName));
            services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));
            services.Configure<EmailOptions>(config.GetSection(EmailOptions.SectionName));
            services.Configure<GoogleOptions>(config.GetSection(GoogleOptions.SectionName));
            
            services.AddProblemDetails();
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<ConflictExceptionHandler>();
            services.AddExceptionHandler<ForbiddenExceptionHandler>();
            services.AddExceptionHandler<AccountLockedExceptionHandler>();
            services.AddExceptionHandler<EmailNotConfirmedExceptionHandler>();
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<PostgresExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddAuthentication();
            services.AddAuthorization();
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(30);
            });
            services.ConfigureHttpJsonOptions(options => {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddCarter();   
            return services;
        }
    }
}