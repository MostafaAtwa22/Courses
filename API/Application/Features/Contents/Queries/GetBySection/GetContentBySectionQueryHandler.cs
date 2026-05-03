using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetBySection
{
    public sealed class GetContentBySectionQueryHandler(IContentRepository _repo) : IRequestHandler<GetContentBySectionQuery, IReadOnlyList<ContentResponseDto>>
    {
        public async Task<IReadOnlyList<ContentResponseDto>> Handle(GetContentBySectionQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetBySectionAsync(request.SectionId, cancellationToken);
        }
    }
}
