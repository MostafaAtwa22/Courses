using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;
using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Features.Categories.Commands.Update
{
    public sealed class UpdateCategoryCommandHandler(
        ICategoryRepository _repo,
        IAppCache _cache) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.Id);

            request.Dto.UpdateEntity(category);

            await _repo.UpdateAsync(category, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Categories(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Category(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        }
    }
}