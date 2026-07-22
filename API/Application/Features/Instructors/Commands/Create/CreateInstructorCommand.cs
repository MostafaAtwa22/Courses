using Application.DTOs.Instructor;
using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Application.Features.Instructors.Commands.Create
{
    public sealed record CreateInstructorCommand(InstructorCreateDto Dto) : IRequest<Guid>, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
