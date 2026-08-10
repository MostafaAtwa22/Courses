using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Commands.MarkComplete
{
    public sealed record MarkContentCompleteCommand(MarkProgressRequestDto Dto) 
        : IRequest, IRequireStudent, IRequireEnrollment, IRequireContentCourseValidation, IStudentInjectable
    {
        public Guid CourseId => Dto.CourseId;
        public Guid ContentId => Dto.ContentId;
        public bool AllowPreview => false;
        public Guid StudentId { get; set; }
    }
}
