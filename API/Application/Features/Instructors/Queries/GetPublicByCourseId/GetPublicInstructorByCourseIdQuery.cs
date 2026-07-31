using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicByCourseId
{
    public sealed record GetPublicInstructorByCourseIdQuery(Guid CourseId) : IRequest<InstructorPublicResponseDto?>;
}
