using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Application.Features.Account.Commands.UnLock
{
    public sealed class UnLockUserCommandHandler(
        IAuthService _authService,
        IIdentityEmailService _identityEmailService) 
        : IRequestHandler<UnLockUserCommand>
    {
        public async Task Handle(UnLockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);
            
            var result = await _authService.UnLockUserAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to unlock the user account.");

            await _authService.ResetAccessFailedCountAsync(user);

            await _identityEmailService.SendAccountUnlockedEmailAsync(user);
        }
    }
}