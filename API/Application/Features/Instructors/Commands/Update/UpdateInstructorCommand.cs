using Application.DTOs.Instructor;
using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Application.Features.Instructors.Commands.Update
{
    public sealed record UpdateInstructorCommand(Guid Id, InstructorUpdateDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
