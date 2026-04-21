using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryResponseDto>>;
}