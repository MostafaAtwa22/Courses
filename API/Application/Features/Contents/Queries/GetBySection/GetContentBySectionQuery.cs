using Application.Common.Models;
using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Queries.GetBySection
{
    public sealed record GetContentBySectionQuery(Guid SectionId) : IRequest<IReadOnlyList<ContentResponseDto>>;
}
