using Application.Common.Interfaces.Identity;
using Infrastructure.Constants;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Permissions
{
    public class PermissionService(ApplicationDbContext _context) : IPermissionService
    {
        public async Task<HashSet<string>> GetPermissionsAsync(string userId)
        {
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => _context.RoleClaims
                    .Where(rc => rc.RoleId == ur.RoleId && rc.ClaimValue != null))
                .Where(cv => cv.ClaimValue!.StartsWith(PermissionConstants.PermissionClaimValuePrefix))
                .Select(cv => cv.ClaimValue!)
                .Distinct()
                .ToListAsync();
            
            return permissions.ToHashSet()!;
        }
    }
}