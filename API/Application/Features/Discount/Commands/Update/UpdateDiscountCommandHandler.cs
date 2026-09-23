using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Exceptions;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Discount.Commands.Update
{
    public sealed class UpdateDiscountCommandHandler(
        ICourseDiscountRepository _discountRepository,
        IAppCache _cache) : IRequestHandler<UpdateDiscountCommand>
    {
        public async Task Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _discountRepository.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("CourseDiscount", request.Id);

            request.Dto.UpdateEntity(discount);

            await _discountRepository.UpdateAsync(discount, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Discounts(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.DiscountsByCourse(discount.CourseId), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Course(discount.CourseId), cancellationToken);
        }
    }
}
