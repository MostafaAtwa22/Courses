using Domain.Entities.Identity;

namespace Application.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        IUserIdentityService _userIdentityService,
        IInstructorRepository _instructorRepo)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequireAuthorization
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            var user = await _userIdentityService.FindUserByIdAsync(userId)
                ?? throw new UnauthorizedException("User not found.");

            // Check if user has any of the required roles
            bool hasRequiredRole = await HasAnyRequiredRoleAsync(user, request.RequiredRoles);

            // If user has required role, allow access
            if (hasRequiredRole)
                return await next();

            // If ownership verification is required, check if user owns the resource
            if (request.RequireOwnership)
            {
                var instructor = await _instructorRepo.GetByUserIdAsync(userId, cancellationToken);
                if (instructor == null || instructor.Id != request.ResourceId)
                {
                    throw new ForbiddenException("You are not authorized to access this resource.");
                }
                return await next();
            }

            // User doesn't have required role and ownership is not required/relevant
            throw new ForbiddenException("You are not authorized to access this resource.");
        }

        private async Task<bool> HasAnyRequiredRoleAsync(ApplicationUser user, string[] requiredRoles)
        {
            if (requiredRoles == null || requiredRoles.Length == 0)
                return false;

            foreach (var role in requiredRoles)
            {
                if (await _userIdentityService.IsInRoleAsync(user, role))
                    return true;
            }

            return false;
        }
    }
}
