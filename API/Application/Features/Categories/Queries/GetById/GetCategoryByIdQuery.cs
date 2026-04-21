using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetById
{
    public sealed record GetCategoryByIdQuery(Guid id) : IRequest<CategoryResponseDto?>;
}