using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetByInstructorId
{
    public sealed record GetCoursesByInstructorIdQuery(CourseQueryParams QueryParams) 
        : IRequest<PaginatedResult<CourseSummaryDto>>, IRequireInstructor, IInstructorInjectable
    {
        public Guid InstructorId { get; set; }
    }
}
