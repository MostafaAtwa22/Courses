using Domain.Entities.Identity;

namespace Application.Features.Security.Commands.Enable2FA
{
    public sealed record Enable2FACommand(string Code) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
