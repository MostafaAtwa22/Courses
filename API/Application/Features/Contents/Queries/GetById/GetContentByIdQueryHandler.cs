using Application.Common.Interfaces;
using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Queries.GetById
{
    public sealed class GetContentByIdQueryHandler(IContentRepository _repo) : IRequestHandler<GetContentByIdQuery, ContentResponseDto?>
    {
        public async Task<ContentResponseDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
