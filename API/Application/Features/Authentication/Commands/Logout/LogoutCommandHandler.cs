using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository _refreshTokenRepository) :
    IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.Token) 
            ?? throw new UnauthorizedException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token is already expired or revoked");

        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);
    }
}
