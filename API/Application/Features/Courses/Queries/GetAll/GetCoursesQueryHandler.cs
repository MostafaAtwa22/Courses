using Application.Common.Interfaces;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetAll
{
    public sealed class GetCoursesQueryHandler(ICourseRepository _repo)
        : IRequestHandler<GetCoursesQuery, PaginatedResult<CoursesResponseDto>>
    {
        public Task<PaginatedResult<CoursesResponseDto>> Handle(GetCoursesQuery request, CancellationToken ct)
        {
            return _repo.GetAllAsync(request.QueryParams, ct);
        }
    }
}