using Application.DTOs.Section;

namespace Application.Features.Sections.Queries.GetAll
{
    public sealed class GetSectionsQueryHandler(ISectionRepository _repo) : IRequestHandler<GetSectionsQuery, PaginatedResult<SectionResponseDto>>
    {
        public async Task<PaginatedResult<SectionResponseDto>> Handle(GetSectionsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(request.QueryParams, cancellationToken);
        }
    }
}
