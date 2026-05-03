using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Queries.GetById
{
    public sealed record GetContentByIdQuery(Guid Id) : IRequest<ContentResponseDto?>;
}
