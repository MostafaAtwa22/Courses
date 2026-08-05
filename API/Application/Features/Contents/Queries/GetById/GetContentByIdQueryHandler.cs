using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetById
{
    public sealed class GetContentByIdQueryHandler(
        IContentRepository _repo) 
        : IRequestHandler<GetContentByIdQuery, ContentResponseDto?>
    {
        public async Task<ContentResponseDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetByIdAsync(request.Id, cancellationToken);
            return content;
        }
    }
}
