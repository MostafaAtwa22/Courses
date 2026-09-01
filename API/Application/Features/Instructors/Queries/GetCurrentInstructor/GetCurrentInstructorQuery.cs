using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetCurrentInstructor
{
    public sealed record GetCurrentInstructorQuery : IRequest<InstructorPrivateResponseDto?>;
}
