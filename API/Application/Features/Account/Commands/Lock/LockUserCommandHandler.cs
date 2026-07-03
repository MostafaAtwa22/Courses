using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using Domain.Enums.Identity;

namespace Application.Features.Account.Commands.Lock
{
    public sealed class LockUserCommandHandler(
        IUserIdentityService _userIdentityService,
        IPasswordService _passwordService,
        IIdentityEmailService _identityEmailService) 
        : IRequestHandler<LockUserCommand>
    {
        public async Task Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.FindUserByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);
            
            if (await _userIdentityService.IsInRoleAsync(user, Role.SuperAdmin.ToString()))
                throw new BadRequestException("Cannot lock a SuperAdmin account.");

            var lockUntil = request.Dto.LockoutUntil.HasValue ? request.Dto.LockoutUntil.Value.UtcDateTime : DateTimeOffset.MaxValue.UtcDateTime;
            
            var result = await _passwordService.LockUserAsync(user, lockUntil);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to lock the user account.");
            
            await _userIdentityService.ResetAccessFailedCountAsync(user);

            await _identityEmailService.SendAccountLockedEmailAsync(user, request.Dto);
        }
    }
}