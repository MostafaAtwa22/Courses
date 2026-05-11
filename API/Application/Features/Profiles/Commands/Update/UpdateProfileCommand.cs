using Application.DTOs.Profile;
using Domain.Entities.Identity;

namespace Application.Features.Profiles.Commands.Update
{
    public sealed record UpdateProfileCommand(UpdateProfileDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
