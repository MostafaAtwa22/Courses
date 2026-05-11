using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Behaviors
{
    public class UserContextBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        UserManager<ApplicationUser> _userManager)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICurrentUserRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("Login your account first");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(userId));

            request.User = user;

            return await next();
        }
    }
}
