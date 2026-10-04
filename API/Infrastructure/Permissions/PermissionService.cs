using Application.Common.Interfaces.Identity;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Permissions
{
    public class PermissionService(ApplicationDbContext _context) : IPermissionService
    {
        public async Task<HashSet<string>> GetPermissionsAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user is null) return [];

            return user.Roles
                .SelectMany(r => r.Permissions)
                .Select(p => p.Name)
                .ToHashSet();
        }
    }
}