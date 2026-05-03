using Application.Common.Models;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetByCourseId
{
    public sealed record GetSectionsByCourseIdQuery(Guid CourseId, QueryParams QueryParams) : IRequest<PaginatedResult<SectionResponseDto>>;
}
