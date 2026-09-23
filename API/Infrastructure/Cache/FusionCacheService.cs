using Application.Common.Interfaces.Cache;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Cache
{
    public class FusionCacheService(HybridCache _cache) : IAppCache
    {
        public async ValueTask<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T?>> factory, CacheOptions? options = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
        {
            var entryOptions = options is null
                ? null
                : new HybridCacheEntryOptions
                {
                    Expiration = options.Expiration,
                    LocalCacheExpiration = options.LocalCacheExpiration
                };

            return await _cache.GetOrCreateAsync(
                key,
                factory,
                entryOptions,
                tags,
                cancellationToken
            );
        }

        public async ValueTask SetAsync<T>(string key, T data, CacheOptions? options = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
        {
            var entryOptions = options is null
                ? null
                : new HybridCacheEntryOptions
                {
                    Expiration = options.Expiration,
                    LocalCacheExpiration = options.LocalCacheExpiration
                };

            await _cache.SetAsync(
                key,
                data,
                entryOptions,
                tags,
                cancellationToken
            );
        }

        public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
            => _cache.RemoveAsync(
                key,
                cancellationToken
            );

        public ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
            => _cache.RemoveByTagAsync(
                tag,
                cancellationToken
            );
    }
}