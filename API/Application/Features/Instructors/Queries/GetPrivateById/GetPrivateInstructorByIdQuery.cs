using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed record GetPrivateInstructorByIdQuery(Guid Id) : IRequest<InstructorPrivateResponseDto?>;
}
