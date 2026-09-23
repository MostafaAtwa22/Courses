using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using MediatR;

namespace Application.Features.Discount.Queries.GetDiscounts
{
    public sealed class GetCourseDiscountsQueryHandler(
        ICourseDiscountRepository _discountRepository,
        IAppCache _cache) : IRequestHandler<GetCourseDiscountsQuery, IEnumerable<CourseDiscountDto>>
    {
        public async Task<IEnumerable<CourseDiscountDto>> Handle(GetCourseDiscountsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.DiscountsByCourse(request.CourseId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(10),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _discountRepository.GetByCourseIdAsync(request.CourseId, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Discounts(), CacheKeys.Courses() },
                cancellationToken);

            return result ?? Array.Empty<CourseDiscountDto>();
        }
    }
}
