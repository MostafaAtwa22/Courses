using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IPasswordService
{
    Task<bool> ConfirmEmailAsync(ApplicationUser user, string code);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    Task<IdentityResult> SetPasswordAsync(ApplicationUser user, string newPassword);
    Task<bool> HasPasswordAsync(ApplicationUser user);
    Task<IdentityResult> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);
    Task<IdentityResult> UnLockUserAsync(ApplicationUser user);
}
