using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicById
{
    public sealed record GetPublicInstructorByIdQuery(Guid Id) : IRequest<InstructorPublicResponseDto?>;
}
