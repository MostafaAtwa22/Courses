using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IIdentityEmailService
    {
        Task SendConfirmationEmailAsync(ApplicationUser user);
    }
}
