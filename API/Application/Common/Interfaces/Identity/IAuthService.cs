using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUserNameExistsAsync(string userName);
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}