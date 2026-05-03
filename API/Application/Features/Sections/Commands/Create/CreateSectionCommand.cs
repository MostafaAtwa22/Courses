using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Commands.Create
{
    public sealed record CreateSectionCommand(SectionCreateDto Dto) : IRequest<Guid>;
}
