using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface ICurrentUserRequest
    {
        ApplicationUser? User { get; set; }
    }
}
