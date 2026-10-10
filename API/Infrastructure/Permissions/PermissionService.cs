using Application.Common.Interfaces.Identity;
using Domain.Constants;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Permissions
{
    public class PermissionService(ApplicationDbContext _context) : IPermissionService
    {
        public async Task<HashSet<string>> GetPermissionsAsync(string userId)
        {
            var permissions = 
            await (from ur in _context.UserRoles
                join rc in _context.RoleClaims
                    on ur.RoleId equals rc.RoleId
                where ur.UserId == userId
                    && rc.ClaimValue != null
                    && rc.ClaimValue.StartsWith(PermissionConstants.PermissionClaimValuePrefix)
                select rc.ClaimValue)
                .Distinct()
                .ToListAsync();

            return permissions.ToHashSet()!;
        }
    }
}