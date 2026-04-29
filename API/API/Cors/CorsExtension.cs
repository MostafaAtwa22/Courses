using Application.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Cors
{
    public static class CorsExtension
    {
        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            var urlsOptions = configuration.GetSection("Urls").Get<UrlsOptions>()
                ?? new UrlsOptions();

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.WithOrigins(urlsOptions.Client)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
