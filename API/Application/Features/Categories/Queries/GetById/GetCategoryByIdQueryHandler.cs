using Application.Common.Interfaces;
using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetById
{
    public sealed class GetCategoryByIdQueryHandler(ICategoryRepository _repo) : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto?>
    {

        public async Task<CategoryResponseDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetByIdAsync(request.id, cancellationToken);
        }
    }
}