using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetBySection
{
    public sealed class GetContentBySectionQueryHandler(IContentRepository _repo) : IRequestHandler<GetContentBySectionQuery, PaginatedResult<ContentResponseDto>>
    {
        public async Task<PaginatedResult<ContentResponseDto>> Handle(GetContentBySectionQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetBySectionAsync(request.SectionId, request.QueryParams, cancellationToken);
        }
    }
}
