using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetById
{
    public sealed record GetSectionByIdQuery(Guid Id) : IRequest<SectionResponseDto?>;
}
