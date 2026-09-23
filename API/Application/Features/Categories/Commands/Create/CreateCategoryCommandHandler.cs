using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using MediatR;
using Application.Common.Mappings;

namespace Application.Features.Categories.Commands.Create
{
    public sealed class CreateCategoryCommandHandler(
        ICategoryRepository _repo,
        IAppCache _cache) : IRequestHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = request.Dto.ToEntity();
            var categoryId = await _repo.CreateAsync(category, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Categories(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);

            return categoryId;
        }
    }
}