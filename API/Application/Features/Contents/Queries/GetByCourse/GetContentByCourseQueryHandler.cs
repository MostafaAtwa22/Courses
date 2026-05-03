using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetByCourse
{
    public sealed class GetContentByCourseQueryHandler(IContentRepository _repo) : IRequestHandler<GetContentByCourseQuery, PaginatedResult<ContentResponseDto>>
    {
        public async Task<PaginatedResult<ContentResponseDto>> Handle(GetContentByCourseQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByCourseAsync(request.CourseId, request.QueryParams, cancellationToken);
        }
    }
}
