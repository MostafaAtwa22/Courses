using Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;
using Constant = Domain.Constants;

namespace Infrastructure.Identity
{
    public class TwoFactorService(
        UserManager<ApplicationUser> _userManager,
        IIdentityEmailService _emailService
    ) : ITwoFactorService
    {
        public async Task SendOtpAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateTwoFactorTokenAsync(
                user, Constant.IdentityConstants.EmailOtpProvider
            );

            await _emailService.Send2FAEmailAsync(user, code);
        }

        public async Task<bool> VerifyOtpAsync(ApplicationUser user, string code)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(
                user, Constant.IdentityConstants.EmailOtpProvider, code
            );
        }

        public async Task<bool> IsTwoFactorEnabledAsync(ApplicationUser user)
            => await _userManager.GetTwoFactorEnabledAsync(user);
    }
}