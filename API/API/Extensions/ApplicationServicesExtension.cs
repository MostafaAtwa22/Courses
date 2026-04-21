using API.Exceptions;

namespace API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<ConflictExceptionHandler>();
            services.AddExceptionHandler<ForbiddenExceptionHandler>();
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }
    }
}