using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Domain.Entities.Identity;
using IdentityConstants = Domain.Constants.IdentityConstants;

namespace Infrastructure.Identity;

public class LoginPipeline(
    IUserIdentityService _userIdentityService,
    ITwoFactorService _twoFactorService,
    ITokenService _tokenService) : ILoginPipeline
{
    public async Task<AuthResponseDto> ExecuteAsync(ApplicationUser user)
    {
        if (await _userIdentityService.IsLockedOutAsync(user))
            throw new AccountLockedException("Account is locked. Please try again later.");

        if (await _twoFactorService.IsTwoFactorEnabledAsync(user))
        {
            await _twoFactorService.SendOtpAsync(user);
            return new AuthResponseDto
            {
                Email              = user.Email!,
                RequiresTwoFactor  = true,
                Provider           = IdentityConstants.EmailOtpProvider
            };
        }

        return await _tokenService.GenerateAuthWithRefreshTokenAsync(user);
    }
}
