using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;

namespace Application.Features.Courses.Queries.GetSuggestions
{
    public class GetCourseSuggestionsQueryHandler(
        ICourseRepository repository,
        IAppCache _cache) 
        : IRequestHandler<GetCourseSuggestionsQuery, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(GetCourseSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Term))
                return [];

            var cacheKey = CacheKeys.CourseSuggestions(request.Term);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(10),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await repository.GetSuggestionsAsync(request.Term, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Courses() },
                cancellationToken);

            return result ?? Enumerable.Empty<string>();
        }
    }
}
