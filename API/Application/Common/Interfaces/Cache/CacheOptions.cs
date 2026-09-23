namespace Application.Common.Interfaces.Cache
{
    public record CacheOptions(TimeSpan Expiration, TimeSpan LocalCacheExpiration);
}