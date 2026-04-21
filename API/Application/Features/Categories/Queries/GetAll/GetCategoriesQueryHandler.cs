using Application.Common.Interfaces;
using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed class GetCategoriesQueryHandler (ICategoryRepository _repo)
        : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponseDto>>
    {

        public async Task<IReadOnlyList<CategoryResponseDto>> 
            Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(cancellationToken);
        }
    }
}