using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Domain.Constants;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Permissions
{
    public class PermissionService(ApplicationDbContext _context, IAppCache _cache) : IPermissionService
    {
        public async Task<HashSet<string>> GetPermissionsAsync(string userId)
        {
            var cacheKey = CacheKeys.PermissionsByUser(userId);
            var options = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(10),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await FetchFromDbAsync(userId, ct),
                options,
                tags: [CacheKeys.Permissions()]);

            return result?.ToHashSet() ?? [];
        }

        private async Task<List<string>> FetchFromDbAsync(string userId, CancellationToken ct)
        {
            return await (
                from ur in _context.UserRoles
                join rc in _context.RoleClaims on ur.RoleId equals rc.RoleId
                where ur.UserId == userId
                    && rc.ClaimValue != null
                    && rc.ClaimValue.StartsWith(PermissionConstants.PermissionClaimValuePrefix)
                select rc.ClaimValue
            ).Distinct().ToListAsync(ct);
        }
    }
}