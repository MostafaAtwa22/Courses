using Application.Common.Interfaces.Identity;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetMyCoursesProgress
{
    public sealed record GetMyCoursesProgressQuery 
        : IRequest<IReadOnlyList<CourseProgressSummaryDto>>, IRequireStudent, IStudentInjectable
    {
        public Guid StudentId { get; set; }
    }
}
