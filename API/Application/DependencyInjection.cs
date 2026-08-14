using Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(assembly);
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
                options.AddOpenBehavior(typeof(UserContextBehavior<,>));
                options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
                options.AddOpenBehavior(typeof(InstructorOwnershipBehavior<,>));
                options.AddOpenBehavior(typeof(InstructorAuthenticationBehavior<,>));
                options.AddOpenBehavior(typeof(StudentAuthenticationBehavior<,>));
                options.AddOpenBehavior(typeof(EnrollmentAuthorizationBehavior<,>));
                options.AddOpenBehavior(typeof(ContentAccessBehavior<,>));
                options.AddOpenBehavior(typeof(ContentCourseValidationBehavior<,>));
            });
            
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}