using Application.DTOs.Authentication;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUserNameExistsAsync(string userName);
        Task<string> CreateTokenAsync(ApplicationUser user);
        RefreshToken GenerateRefreshToken(string jwtId);
        Task<AuthResponseDto> GetAuthResponseAsync(ApplicationUser user);
        Task AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<ApplicationUser?> FindUserByEmailAsync(string email);
        Task<ApplicationUser?> FindUserByIdAsync(string id);
        Task<bool> IsLockedOutAsync(ApplicationUser user);
        Task RecordFailedAccessAsync(ApplicationUser user);
        Task<bool> ConfirmEmailAsync(ApplicationUser user, string code);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<IdentityResult> SetPasswordAsync(ApplicationUser user, string newPassword);
        Task<bool> HasPasswordAsync(ApplicationUser user);
        Task<IdentityResult> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);
        Task<IdentityResult> UnLockUserAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task UpdateSecurityStampAsync(ApplicationUser user);
        Task ResetAccessFailedCountAsync(ApplicationUser user);
    }
}