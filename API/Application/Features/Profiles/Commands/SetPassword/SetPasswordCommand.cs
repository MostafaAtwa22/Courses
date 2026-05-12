using Application.DTOs.Profile;
using Domain.Entities.Identity;

namespace Application.Features.Profiles.Commands.SetPassword
{
    public sealed record SetPasswordCommand(SetPasswordDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
