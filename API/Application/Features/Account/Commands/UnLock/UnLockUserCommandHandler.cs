using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Application.Features.Account.Commands.UnLock
{
    public sealed class UnLockUserCommandHandler(
        IUserIdentityService _userIdentityService,
        IPasswordService _passwordService,
        IIdentityEmailService _identityEmailService) 
        : IRequestHandler<UnLockUserCommand>
    {
        public async Task Handle(UnLockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.FindUserByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);
            
            var result = await _passwordService.UnLockUserAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to unlock the user account.");

            await _userIdentityService.ResetAccessFailedCountAsync(user);

            await _identityEmailService.SendAccountUnlockedEmailAsync(user);
        }
    }
}