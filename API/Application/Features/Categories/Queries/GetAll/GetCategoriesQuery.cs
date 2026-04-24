using Application.Common.Models;
using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed record GetCategoriesQuery(QueryParams Params) : IRequest<PaginatedResult<CategoryResponseDto>>;
}