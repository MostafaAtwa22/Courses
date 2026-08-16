using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Behaviors
{
    public class ContentCourseValidationBehavior<TRequest, TResponse>(
        IEnrollmentRepository _enrollmentRepo)
        : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequireContentCourseValidation
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request.ContentId != Guid.Empty)
            {
                var courseIdForContent = await _enrollmentRepo.GetCourseIdByContentIdAsync(request.ContentId, cancellationToken);
                if (courseIdForContent != request.CourseId)
                    throw new BadRequestException("Content does not belong to the specified course.");
            }

            return await next();
        }
    }
}
