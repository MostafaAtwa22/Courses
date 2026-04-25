using API.Exceptions;
using Application.Common.Options;

namespace API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<UrlsOptions>(config.GetSection("Urls"));

            services.AddProblemDetails();
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<ConflictExceptionHandler>();
            services.AddExceptionHandler<ForbiddenExceptionHandler>();
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<PostgresExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddCarter();
            return services;
        }
    }
}