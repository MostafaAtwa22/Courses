using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetByCourseId
{
    public sealed class GetSectionsByCourseIdQueryHandler(ISectionRepository _repo) : IRequestHandler<GetSectionsByCourseIdQuery, PaginatedResult<SectionResponseDto>>
    {
        public async Task<PaginatedResult<SectionResponseDto>> Handle(GetSectionsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByCourseIdAsync(request.CourseId, request.QueryParams, cancellationToken);
        }
    }
}
