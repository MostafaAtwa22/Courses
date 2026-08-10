namespace Application.Behaviors
{
    public class EnrollmentAuthorizationBehavior<TRequest, TResponse>(
        IContentAccessService _contentAccessService,
        IContentRepository _contentRepository)
        : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequireEnrollment
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request.CourseId == Guid.Empty)
                return await next();

            if (request.ContentId == Guid.Empty)
            {
                var hasEnrollment = await _contentAccessService.HasFullCourseContentAccessAsync(request.CourseId, cancellationToken);
                if (!hasEnrollment)
                    throw new ForbiddenException("You must be enrolled in this course to access this content.");
                return await next();
            }

            var hasFullAccess = await _contentAccessService.HasFullCourseContentAccessAsync(request.CourseId, cancellationToken);
            
            if (hasFullAccess)
                return await next();

            if (request.AllowPreview)
            {
                var content = await _contentRepository.GetEntityByIdAsync(request.ContentId, cancellationToken);
                if (content != null && content.IsPreview)
                    return await next();
            }

            throw new ForbiddenException("You must be enrolled in this course to access this content.");
        }
    }
}
