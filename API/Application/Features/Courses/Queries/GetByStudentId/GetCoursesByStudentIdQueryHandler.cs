namespace Application.Features.Courses.Queries.GetByStudentId
{
    public sealed class GetCoursesByStudentIdQueryHandler(
        ICourseRepository _repo, 
        IStudentRepository _studentRepo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<GetCoursesByStudentIdQuery, PaginatedResult<CourseSummaryDto>>
    {
        public async Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByStudentIdQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            
            var studentId = await _studentRepo.GetStudentIdByUserIdAsync(userId, ct)
                ?? throw new NotFoundException("Student", Guid.Parse(userId));
            
            return await _repo.GetCoursesByStudentIdAsync(studentId, request.QueryParams, ct);
        }
    }
}
