using Domain.Entities.Identity;

namespace Application.Features.Security.Commands.Generate2FA
{
    public sealed record Generate2FATokenCommand() : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
