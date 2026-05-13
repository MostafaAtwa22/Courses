using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Security.Commands.Enable2FA
{
    public sealed class Enable2FACommandHandler(
        UserManager<ApplicationUser> _userManager,
        ITwoFactorService _twoFactorService) 
        : IRequestHandler<Enable2FACommand>
    {
        public async Task Handle(Enable2FACommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if (user.TwoFactorEnabled)
                throw new BadRequestException("2FA is already enabled for this user.");
            
            if (!user.EmailConfirmed)
                throw new BadRequestException("Email must be confirmed before enabling 2FA.");

            var isTokenValid = await _twoFactorService.VerifyOtpAsync(user, request.Code);
            if (!isTokenValid)
                throw new BadRequestException("Invalid 2FA code.");
    
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to enable 2FA.");
        }
    }
}
