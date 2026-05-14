namespace Application.Behaviors
{
    public class InstructorOwnershipBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        IInstructorRepository _instructorRepo,
        ICourseRepository _courseRepo,
        ISectionRepository _sectionRepo,
        IContentRepository _contentRepo)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IInstructorOwnedRequest
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            var instructor = await _instructorRepo.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only instructors can perform this action.");

            Guid instructorIdToVerify = Guid.Empty;

            switch (request.ResourceType)
            {
                case ResourceType.Course:
                    var course = await _courseRepo.GetEntityByIdAsync(request.Id, cancellationToken)
                        ?? throw new NotFoundException(nameof(Course), request.Id);
                    instructorIdToVerify = course.InstructorId;
                    break;

                case ResourceType.Section:
                    var section = await _sectionRepo.GetEntityByIdAsync(request.Id, cancellationToken)
                        ?? throw new NotFoundException(nameof(Section), request.Id);
                    var parentCourse = await _courseRepo.GetEntityByIdAsync(section.CourseId, cancellationToken)
                        ?? throw new NotFoundException(nameof(Course), section.CourseId);
                    instructorIdToVerify = parentCourse.InstructorId;
                    break;

                case ResourceType.Content:
                    var content = await RepoGetContentAsync(request.Id, cancellationToken);
                    instructorIdToVerify = content.InstructorId;
                    break;
            }

            if (instructorIdToVerify != instructor.Id)
                throw new ForbiddenException($"You are not authorized to modify this {request.ResourceType.ToString().ToLower()}.");

            return await next();
        }

        private async Task<(Guid InstructorId, Guid ContentId)> RepoGetContentAsync(Guid contentId, CancellationToken ct)
        {
             var content = await _contentRepo.GetEntityByIdAsync(contentId, ct)
                        ?? throw new NotFoundException(nameof(Content), contentId);
             var section = await _sectionRepo.GetEntityByIdAsync(content.SectionId, ct)
                        ?? throw new NotFoundException(nameof(Section), content.SectionId);
             var course = await _courseRepo.GetEntityByIdAsync(section.CourseId, ct)
                        ?? throw new NotFoundException(nameof(Course), section.CourseId);
             
             return (course.InstructorId, content.Id);
        }
    }
}
