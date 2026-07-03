using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IUserIdentityService
{
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsUserNameExistsAsync(string userName);
    Task<ApplicationUser?> FindUserByEmailAsync(string email);
    Task<ApplicationUser?> FindUserByIdAsync(string id);
    Task<bool> IsLockedOutAsync(ApplicationUser user);
    Task RecordFailedAccessAsync(ApplicationUser user);
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
    Task UpdateSecurityStampAsync(ApplicationUser user);
    Task ResetAccessFailedCountAsync(ApplicationUser user);
}
