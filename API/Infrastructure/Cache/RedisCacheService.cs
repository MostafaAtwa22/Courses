using System.Text.Json;
using StackExchange.Redis;

namespace Infrastructure.Cache
{
    public sealed class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDatabase _database = redis.GetDatabase();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            var data = await _database.StringGetAsync(key);

            return data.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<T>((string)data!, _jsonOptions);
        }

        public async Task<T?> UpdateOrCreateAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
        {
            var ttl = expiry ?? TimeSpan.FromMinutes(30);

            var created = await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(value, _jsonOptions),
                ttl);

            return created ? value : null;
        }

        public async Task DeleteAsync(string key, CancellationToken ct = default)
            => await _database.KeyDeleteAsync(key);

        public async Task DeleteByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            var server = redis.GetServer(redis.GetEndPoints().First());
            var keys = server.KeysAsync(pattern: $"{prefix}*");

            await foreach (var key in keys.WithCancellation(ct))
                await _database.KeyDeleteAsync(key);
        }
    }
}