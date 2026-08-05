using Application.DTOs.Content;
using Application.Extensions;

namespace Application.Behaviors
{
    public class ContentAccessBehavior<TRequest, TResponse>(
        IContentAccessService _contentAccessService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequireContentAccess
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var hasFullAccess = await _contentAccessService.HasFullCourseContentAccessAsync(request.CourseId, cancellationToken);

            switch (response)
            {
                case ContentResponseDto content:
                    content.RedactProtectedUrl(hasFullAccess);
                    break;
                case IEnumerable<ContentResponseDto> contents:
                    contents.RedactProtectedUrls(hasFullAccess);
                    break;
                case PaginatedResult<ContentResponseDto> paginatedResult:
                    paginatedResult.Items.RedactProtectedUrls(hasFullAccess);
                    break;
            }

            return response;
        }
    }
}
