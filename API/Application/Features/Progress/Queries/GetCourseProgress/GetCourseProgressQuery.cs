using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed record GetCourseProgressQuery(Guid CourseId) 
        : IRequest<CourseProgressDto>, IRequireStudent, IRequireEnrollment, IStudentInjectable
    {
        public Guid ContentId => Guid.Empty; // Not applicable for course-level queries
        public bool AllowPreview => false;
        public Guid StudentId { get; set; }
    }
}
