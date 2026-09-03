using Application.Common.Interfaces;
using Application.DTOs.Authorization;
using Dapper;

namespace Infrastructure.Repositories
{
    public class RoleRepository(IDbConnectionFactory factory) : BaseRepository(factory), IRoleRepository
    {
        public async Task<IReadOnlyCollection<RolesResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            
            var sql = @"
                SELECT r.id, r.name, COUNT(ur.user_id) as UserCount
                FROM ""AspNetRoles"" r
                LEFT JOIN ""AspNetUserRoles"" ur ON r.id = ur.role_id
                GROUP BY r.id, r.name
                ORDER BY r.name";

            var roles = await connection.QueryAsync<RolesResponseDto>(sql);

            return roles.ToList();
        }

        public async Task<IReadOnlyCollection<CheckBoxRoleManageDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            
            var sql = @"
                SELECT
                    r.id AS RoleId,
                    r.name AS RoleName,
                    CASE
                        WHEN ur.user_id IS NOT NULL THEN TRUE
                        ELSE FALSE
                    END AS IsSelected
                FROM ""AspNetRoles"" r
                LEFT JOIN ""AspNetUserRoles"" ur ON ur.role_id = r.id AND ur.user_id = @UserId
                ORDER BY r.name";

            var roles = await connection.QueryAsync<CheckBoxRoleManageDto>(sql, new { UserId = userId });

            return roles.ToList();
        }
    }
}
