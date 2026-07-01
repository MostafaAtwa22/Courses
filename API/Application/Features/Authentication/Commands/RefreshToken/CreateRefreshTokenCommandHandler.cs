using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Features.Authentication.Commands.RefreshToken
{
    public sealed class CreateRefreshTokenCommandHandler(
        IAuthService _authService) : 
        IRequestHandler<CreateRefreshTokenCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _authService.GetRefreshTokenAsync(request.Token);

            if (storedToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (!storedToken.IsActive)
            {
                storedToken.IsRevoked = true;
                await _authService.UpdateRefreshTokenAsync(storedToken);
                throw new UnauthorizedException("Refresh token is expired or revoked");
            }

            var user = storedToken.User;
            if (user == null)
                throw new UnauthorizedException("Invalid refresh token");

            // Mark old token as used
            storedToken.IsUsed = true;
            await _authService.UpdateRefreshTokenAsync(storedToken);

            // Generate new token
            var response = await _authService.GetAuthResponseAsync(user);

            var jwtId = new JwtSecurityTokenHandler()
                .ReadJwtToken(response.Token)
                .Id;

            var newRefreshToken = _authService.GenerateRefreshToken(jwtId);
            await _authService.AddRefreshTokenAsync(user, newRefreshToken);

            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.ExpiryDate;

            return response;
        }
    }
}
