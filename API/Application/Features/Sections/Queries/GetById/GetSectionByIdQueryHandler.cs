using Application.Common.Interfaces;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetById
{
    public sealed class GetSectionByIdQueryHandler(ISectionRepository _repo) : IRequestHandler<GetSectionByIdQuery, SectionResponseDto?>
    {
        public async Task<SectionResponseDto?> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
