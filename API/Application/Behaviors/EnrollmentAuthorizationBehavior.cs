using Domain.Enums.Identity;

namespace Application.Behaviors
{
    public class EnrollmentAuthorizationBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        IUserIdentityService _userIdentityService,
        IEnrollmentRepository _enrollmentRepository,
        IContentRepository _contentRepository,
        IInstructorRepository _instructorRepository)
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

            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            var user = await _userIdentityService.FindUserByIdAsync(userId)
                ?? throw new UnauthorizedException("User not found.");

            if (await _userIdentityService.IsInRoleAsync(user, Role.Admin.ToString()) ||
                await _userIdentityService.IsInRoleAsync(user, Role.SuperAdmin.ToString()))
            {
                return await next();
            }

            // Check if user is the course instructor
            var instructorId = await _enrollmentRepository.GetInstructorIdByCourseIdAsync(request.CourseId, cancellationToken);
            if (instructorId.HasValue)
            {
                var instructor = await _instructorRepository.GetByUserIdAsync(userId, cancellationToken);
                if (instructor != null && instructor.Id == instructorId.Value)
                    return await next();
            }

            // Check if content is preview and preview access is allowed
            if (request.AllowPreview && request.ContentId != Guid.Empty)
            {
                var content = await _contentRepository.GetEntityByIdAsync(request.ContentId, cancellationToken);
                if (content != null && content.IsPreview)
                    return await next();
            }

            // Check if user is enrolled in the course
            var isEnrolled = await _enrollmentRepository.IsEnrolledByUserIdAsync(userId, request.CourseId, cancellationToken);
            if (isEnrolled)
                return await next();

            // User is not authorized to access this content
            throw new ForbiddenException("You must be enrolled in this course to access this content.");
        }
    }
}
