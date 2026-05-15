using Application.Common.Exceptions;

namespace Application.Features.Security.Commands.VerifyTwoFactor
{
    public sealed class VerifyTwoFactorCommandHandler(
            IAuthService _authService,
            ITwoFactorService _twoFactorService) :
        IRequestHandler<VerifyTwoFactorCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("Invalid verification attempt.");

            if (await _authService.IsLockedOutAsync(user))
                throw new AccountLockedException("Your account is locked. Please try again later.");

            if (!user.TwoFactorEnabled)
                throw new UnauthorizedException("Two-factor authentication is not enabled for this account.");

            var isValid = await _twoFactorService.VerifyOtpAsync(user, request.Dto.Code);

            if (!isValid)
            {
                await _authService.RecordFailedAccessAsync(user);
                throw new UnauthorizedException("Invalid verification code.");
            }

            return await _authService.GetAuthResponseAsync(user);
        }
    }
}
