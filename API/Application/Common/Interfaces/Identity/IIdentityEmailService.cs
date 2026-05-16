using Application.DTOs.Account;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IIdentityEmailService
    {
        Task SendEmailConfirmationEmailAsync(ApplicationUser user, string token);
        Task Send2FAEmailAsync(ApplicationUser user, string code);
        Task SendAccountLockedEmailAsync(ApplicationUser user, LockUserDto dto);
        Task SendAccountUnlockedEmailAsync(ApplicationUser user);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string token);
    }
}
