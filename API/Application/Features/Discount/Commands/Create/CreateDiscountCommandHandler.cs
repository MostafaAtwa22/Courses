using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Discount.Commands.Create
{
    public sealed class CreateDiscountCommandHandler(
        ICourseRepository _courseRepository,
        ICourseDiscountRepository _discountRepository,
        IAppCache _cache) : IRequestHandler<CreateDiscountCommand, Guid>
    {
        public async Task<Guid> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetEntityByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course", request.CourseId);

            var discount = request.Dto.ToEntity(request.CourseId);

            var discountId = await _discountRepository.AddAsync(discount, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Discounts(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.DiscountsByCourse(request.CourseId), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Course(request.CourseId), cancellationToken);

            return discountId;
        }
    }
}
