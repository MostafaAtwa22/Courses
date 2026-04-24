using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed class GetCategoriesQueryHandler (ICategoryRepository _repo)
        : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryResponseDto>>
    {

        public async Task<PaginatedResult<CategoryResponseDto>> 
            Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(request.Params, cancellationToken);
        }
    }
}