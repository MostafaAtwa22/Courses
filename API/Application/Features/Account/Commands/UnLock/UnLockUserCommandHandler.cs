using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.UnLock
{
    public sealed class UnLockUserCommandHandler(UserManager<ApplicationUser> _userManager) 
        : IRequestHandler<UnLockUserCommand>
    {
        public async Task Handle(UnLockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);
            
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddSeconds(-1));
            await _userManager.ResetAccessFailedCountAsync(user);
        }
    }
}