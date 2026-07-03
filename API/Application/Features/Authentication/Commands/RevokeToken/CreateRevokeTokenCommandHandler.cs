using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Authentication.Commands.RevokeToken;

public sealed class CreateRevokeTokenCommandHandler(
    IRefreshTokenRepository _refreshTokenRepository) :
    IRequestHandler<CreateRevokeTokenCommand>
{
    public async Task Handle(CreateRevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.Token);

        if (storedToken == null)
            throw new UnauthorizedException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token is already expired or revoked");

        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);
    }
}
