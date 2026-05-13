using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Security.Commands.Disable2FA
{
    public sealed class Disable2FACommandHandler(
        UserManager<ApplicationUser> _userManager,
        ITwoFactorService _twoFactorService) 
        : IRequestHandler<Disable2FACommand>
    {
        public async Task Handle(Disable2FACommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if (!user.TwoFactorEnabled)
                throw new BadRequestException("2FA is already disabled for this user.");
            
            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Dto.Password);
            if (!passwordCheck)
                throw new BadRequestException("Incorrect password.");

            var isTokenValid = await _twoFactorService.VerifyOtpAsync(user, request.Dto.Code);
            if (!isTokenValid)
                throw new BadRequestException("Invalid 2FA code.");
    
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to disable 2FA.");
        }
    }
}
