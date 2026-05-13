using Application.DTOs.Security;
using Domain.Entities.Identity;

namespace Application.Features.Security.Commands.Disable2FA
{
    public sealed record Disable2FACommand(Disable2FADto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
