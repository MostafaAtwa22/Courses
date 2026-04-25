using Application.DTOs.Category;

namespace Application.Features.Categories.Queries.GetById
{
    public sealed record GetCategoryByIdQuery(Guid id) : IRequest<CategoryResponseDto?>;
}