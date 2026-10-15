using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Account.Commands.Lock
{
    public sealed class LockUserCommandHandler(
        IUserIdentityService _userIdentityService,
        IPasswordService _passwordService,
        IIdentityEmailService _identityEmailService,
        IRefreshTokenRepository _refreshTokenRepository,
        IAppCache _cache)
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

            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);
            await _userIdentityService.UpdateSecurityStampAsync(user);

            await _cache.SetAsync(
                CacheKeys.UserLockout(user.Id),
                true,
                new CacheOptions(
                    Expiration: TimeSpan.FromMinutes(5),
                    LocalCacheExpiration: TimeSpan.FromSeconds(30)),
                cancellationToken: cancellationToken);

            await _cache.RemoveByTagAsync(CacheKeys.Users(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Instructors(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Students(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Roles(), cancellationToken);
        }
    }
}