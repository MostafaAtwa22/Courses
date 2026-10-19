using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Authentication.Commands.LogoutAll;

public sealed class LogoutAllCommandHandler(
    ICurrentUserService _currentUserService,
    IRefreshTokenRepository _refreshTokenRepository,
    IUserIdentityService _userIdentityService) :
    IRequestHandler<LogoutAllCommand>
{
    public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");

        var user = await _userIdentityService.FindUserByIdAsync(userId)
            ?? throw new UnauthorizedException("User not found.");

        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);

        await _userIdentityService.UpdateSecurityStampAsync(user);
    }
}
