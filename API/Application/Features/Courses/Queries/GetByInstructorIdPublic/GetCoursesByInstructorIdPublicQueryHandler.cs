namespace Application.Features.Courses.Queries.GetByInstructorIdPublic
{
    public sealed class GetCoursesByInstructorIdPublicQueryHandler(ICourseRepository _repo)
        : IRequestHandler<GetCoursesByInstructorIdPublicQuery, PaginatedResult<CourseSummaryDto>>
    {
        public Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByInstructorIdPublicQuery request, CancellationToken ct)
        {
            return _repo.GetPublishedCoursesByInstructorIdAsync(request.InstructorId, request.QueryParams, ct);
        }
    }
}
