using System.IdentityModel.Tokens.Jwt;

namespace Application.Features.Authentication.Commands.RefreshToken;

public sealed class CreateRefreshTokenCommandHandler(
    IRefreshTokenRepository _refreshTokenRepository,
    ITokenService _tokenService) :
    IRequestHandler<CreateRefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.Token);

        if (storedToken == null)
            throw new UnauthorizedException("Invalid refresh token");

        if (!storedToken.IsActive)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);
            throw new UnauthorizedException("Refresh token is expired or revoked");
        }

        var user = storedToken.User;
        if (user == null)
            throw new UnauthorizedException("Invalid refresh token");

        var oldJwt = new JwtSecurityTokenHandler().ReadJwtToken(request.AccessToken);
        var stampInToken = oldJwt.Claims.FirstOrDefault(c => c.Type == "security_stamp")?.Value;

        if (stampInToken != user.SecurityStamp)
        {
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);
            throw new UnauthorizedException("Session has been invalidated. Please log in again.");
        }

        // Mark old token as used
        storedToken.IsUsed = true;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        return await _tokenService.GenerateAuthWithRefreshTokenAsync(user);
    }
}
