using System.IdentityModel.Tokens.Jwt;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Constant = Domain.Constants.IdentityConstants;

namespace Application.Features.Authentication.Commands.ExternalLogin.Facebook
{
    public sealed class CreateFacebookLoginCommandHandler(
            IExternalAuthService _externalAuthService,
            IAuthService _authService,
            ITwoFactorService _twoFactorService,
            UserManager<ApplicationUser> _userManager) :
        IRequestHandler<CreateFacebookLoginCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(CreateFacebookLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _externalAuthService.FacebookLoginAsync(request.Dto);

            if (await _authService.IsLockedOutAsync(user))
                throw new AccountLockedException("Account is locked. Please try again later.");

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                await _twoFactorService.SendOtpAsync(user);
                return new AuthResponseDto
                {
                    Email = user.Email!,
                    RequiresTwoFactor = true,
                    Provider = Constant.EmailOtpProvider
                };
            }

            var response = await _authService.GetAuthResponseAsync(user);

            var jwtId = new JwtSecurityTokenHandler()
                .ReadJwtToken(response.Token)
                .Id;

            var refreshToken = _authService.GenerateRefreshToken(jwtId);
            await _authService.AddRefreshTokenAsync(user, refreshToken);

            response.RefreshToken = refreshToken.Token;
            response.RefreshTokenExpiration = refreshToken.ExpiryDate;

            return response;
        }
    }
}