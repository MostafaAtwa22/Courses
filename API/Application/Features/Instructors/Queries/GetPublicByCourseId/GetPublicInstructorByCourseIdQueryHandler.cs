using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicByCourseId
{
    public sealed class GetPublicInstructorByCourseIdQueryHandler(IInstructorRepository _repo)
        : IRequestHandler<GetPublicInstructorByCourseIdQuery, InstructorPublicResponseDto?>
    {
        public async Task<InstructorPublicResponseDto?> Handle(
            GetPublicInstructorByCourseIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetPublicByCourseIdAsync(request.CourseId, cancellationToken);
        }
    }
}
