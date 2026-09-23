using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Domain.Entities;
using MediatR;

namespace Application.Features.Discount.Commands.Delete
{
    public sealed class DeleteDiscountCommandHandler(
        ICourseDiscountRepository _discountRepository,
        IAppCache _cache) : IRequestHandler<DeleteDiscountCommand>
    {
        public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _discountRepository.GetEntityByIdAsync(request.Id, cancellationToken);
            if (discount is null)
                throw new NotFoundException("CourseDiscount", request.Id);

            await _discountRepository.DeleteAsync(request.Id, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Discounts(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.DiscountsByCourse(discount.CourseId), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Course(discount.CourseId), cancellationToken);
        }
    }
}
