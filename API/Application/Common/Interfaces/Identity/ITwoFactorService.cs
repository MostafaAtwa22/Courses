using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface ITwoFactorService
    {
        Task SendOtpAsync(ApplicationUser user);
        Task<bool> VerifyOtpAsync(ApplicationUser user, string code);

    }
}