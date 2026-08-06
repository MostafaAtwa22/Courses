using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed record GetCourseProgressQuery(Guid CourseId) : IRequest<CourseProgressDto>;
}
