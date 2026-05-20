using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Behaviors
{
    public class UserContextBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        UserManager<ApplicationUser> _userManager,
        Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICurrentUserRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("Login your account first");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(userId));

            if (user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.UtcNow)
                throw new UnauthorizedException("Your account is locked.");

            var securityStampClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(c => c.Type == "security_stamp")?.Value;
            if (!string.IsNullOrEmpty(user.SecurityStamp) && user.SecurityStamp != securityStampClaim)
                throw new UnauthorizedException("Session has been invalidated. Please login again.");

            request.User = user;

            return await next();
        }
    }
}
