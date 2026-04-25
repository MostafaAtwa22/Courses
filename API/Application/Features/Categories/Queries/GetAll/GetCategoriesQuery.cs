using Application.DTOs.Category;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed record GetCategoriesQuery(QueryParams Params) : IRequest<PaginatedResult<CategoryResponseDto>>;
}