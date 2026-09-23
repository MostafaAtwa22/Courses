using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories.Commands.Delete
{
    public sealed class DeleteCategoryCommandHandler(
        ICategoryRepository _repo,
        IAppCache _cache) : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.Id);

            await _repo.DeleteAsync(request.Id, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Categories(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Category(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        }
    }
}
