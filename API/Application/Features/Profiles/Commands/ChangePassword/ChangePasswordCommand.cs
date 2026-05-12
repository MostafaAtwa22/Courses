using Application.DTOs.Profile;
using Domain.Entities.Identity;

namespace Application.Features.Profiles.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(ChangePasswordDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
