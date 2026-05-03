using Application.Common.Models;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetAll
{
    public sealed record GetSectionsQuery(QueryParams QueryParams) : IRequest<PaginatedResult<SectionResponseDto>>;
}
