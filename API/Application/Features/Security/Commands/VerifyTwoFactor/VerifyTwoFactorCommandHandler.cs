using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Security.Commands.VerifyTwoFactor
{
    public sealed class VerifyTwoFactorCommandHandler(
            UserManager<ApplicationUser> _userManager,
            ITwoFactorService _twoFactorService,
            IAuthService _authService) :
        IRequestHandler<VerifyTwoFactorCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid verification attempt.");

            if (!user.TwoFactorEnabled)
                throw new UnauthorizedAccessException("Two-factor authentication is not enabled for this account.");

            var isValid = await _twoFactorService.VerifyOtpAsync(user, request.Dto.Code);

            if (!isValid)
            {
                await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException("Invalid verification code.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await _authService.CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var returnUser = user.ToAuthResponseDto(roles);
            returnUser.Token = token;
            
            return returnUser;
        }
    }
}
