namespace Application.Common.Interfaces.Cache
{
    public interface IAppCache
    {
        ValueTask<T?> GetOrCreateAsync<T> (
            string key,
            Func<CancellationToken, ValueTask<T?>> factory,
            CacheOptions? options = null,
            IEnumerable<string>? tags = null,
            CancellationToken cancellationToken = default
        );

        ValueTask SetAsync<T>(
            string key,
            T data,
            CacheOptions? options = null,
            IEnumerable<string>? tags = null,
            CancellationToken cancellationToken = default);

        ValueTask RemoveAsync(
            string key,
            CancellationToken cancellationToken = default);

        ValueTask RemoveByTagAsync(
            string tag,
            CancellationToken cancellationToken = default);
    }
}