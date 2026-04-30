namespace Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
        Task<T?> UpdateOrCreateAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class;
        Task DeleteAsync(string key, CancellationToken ct = default);
        Task DeleteByPrefixAsync(string prefix, CancellationToken ct = default);
    }
}