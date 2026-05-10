using Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AuthService(
        UserManager<ApplicationUser> _userManager) : IAuthService
    {
        public async Task<bool> IsEmailExistsAsync(string email)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<bool> IsUserNameExistsAsync(string userName)
            => await _userManager.FindByNameAsync(userName) is not null;
    }
}