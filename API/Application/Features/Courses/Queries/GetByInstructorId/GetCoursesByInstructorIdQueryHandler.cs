using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetByInstructorId
{
    public sealed class GetCoursesByInstructorIdQueryHandler(ICourseRepository _repo)
        : IRequestHandler<GetCoursesByInstructorIdQuery, PaginatedResult<CourseSummaryDto>>
    {
        public Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByInstructorIdQuery request, CancellationToken ct)
        {
            return _repo.GetCoursesByInstructorIdAsync(request.InstructorId, request.QueryParams, ct);
        }
    }
}
