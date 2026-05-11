using Application.DTOs.Profile;
using Domain.Entities.Identity;

namespace Application.Features.Profiles.Commands.Delete
{
    public sealed record DeleteProfileCommand(DeleteProfileDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
