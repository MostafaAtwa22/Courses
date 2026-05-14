using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Account.Commands.Lock
{
    public sealed class LockUserCommandHandler(UserManager<ApplicationUser> _userManager) 
        : IRequestHandler<LockUserCommand>
    {
        public async Task Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);
            
            if (await _userManager.IsInRoleAsync(user, Role.SuperAdmin.ToString()))
                throw new BadRequestException("Cannot lock a SuperAdmin account.");

            await _userManager.SetLockoutEnabledAsync(user, true);

            var lockUntil = request.LockoutUntil.HasValue ? request.LockoutUntil.Value.UtcDateTime : DateTimeOffset.MaxValue.UtcDateTime;
            var result = await _userManager.SetLockoutEndDateAsync(user, lockUntil);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to lock the user account.");
            await _userManager.ResetAccessFailedCountAsync(user);
        }
    }
}