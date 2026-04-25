using Application.Common.Interfaces;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetById
{
    public sealed class GetCourseByIdQueryHandler(ICourseRepository _repo)
        : IRequestHandler<GetCourseByIdQuery, CoursesResponseDto?>
    {
        public async Task<CoursesResponseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}