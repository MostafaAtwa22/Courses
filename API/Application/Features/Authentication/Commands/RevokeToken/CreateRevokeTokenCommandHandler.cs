using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Authentication.Commands.RevokeToken
{
    public sealed class CreateRevokeTokenCommandHandler(IAuthService _authService) : IRequestHandler<CreateRevokeTokenCommand>
    {
        public async Task Handle(CreateRevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _authService.GetRefreshTokenAsync(request.Token);

            if (storedToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (!storedToken.IsActive)
                throw new UnauthorizedException("Refresh token is already expired or revoked");

            storedToken.IsRevoked = true;
            await _authService.UpdateRefreshTokenAsync(storedToken);
        }
    }
}
