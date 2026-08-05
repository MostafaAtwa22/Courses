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
            // Skip enrollment check if CourseId is empty (internal operations)
            if (request.CourseId == Guid.Empty)
            {
                return await next();
            }

            // List queries (ContentId == Guid.Empty) always pass through
            // URL redaction happens in handlers for unauthorized users
            if (request.ContentId == Guid.Empty)
            {
                return await next();
            }

            // Single-item query: enforce strict access control
            var hasFullAccess = await _contentAccessService.HasFullCourseContentAccessAsync(request.CourseId, cancellationToken);
            
            if (hasFullAccess)
            {
                return await next();
            }

            // If user doesn't have full access, check if content is preview and preview is allowed
            if (request.AllowPreview)
            {
                var content = await _contentRepository.GetEntityByIdAsync(request.ContentId, cancellationToken);
                if (content != null && content.IsPreview)
                {
                    return await next();
                }
            }

            // User is not authorized to access this content
            throw new ForbiddenException("You must be enrolled in this course to access this content.");
        }
    }
}
