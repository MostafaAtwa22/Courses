using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;

namespace Application.Features.Security.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandHandler(
    IUserIdentityService _userIdentityService,
    ITwoFactorService _twoFactorService,
    ITokenService _tokenService) :
    IRequestHandler<VerifyTwoFactorCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userIdentityService.FindUserByEmailAsync(request.Dto.Email)
            ?? throw new UnauthorizedException("Invalid verification attempt.");

        if (await _userIdentityService.IsLockedOutAsync(user))
            throw new AccountLockedException("Your account is locked. Please try again later.");

        if (!user.TwoFactorEnabled)
            throw new UnauthorizedException("Two-factor authentication is not enabled for this account.");

        var isValid = await _twoFactorService.VerifyOtpAsync(user, request.Dto.Code);

        if (!isValid)
        {
            await _userIdentityService.RecordFailedAccessAsync(user);
            throw new UnauthorizedException("Invalid verification code.");
        }

        return await _tokenService.GenerateAuthWithRefreshTokenAsync(user);
    }
}
