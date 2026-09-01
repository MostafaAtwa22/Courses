using Application.DTOs.Authorization;

namespace Infrastructure.Repositories
{
    public class RoleRepository(IDbConnectionFactory factory) : BaseRepository(factory), IRoleRepository
    {
        public async Task<IReadOnlyCollection<RoleResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            
            var sql = @"
                SELECT r.Id, r.Name, COUNT(ur.UserId) as UserCount
                FROM ""AspNetRoles"" r
                LEFT JOIN ""AspNetUserRoles"" ur ON r.Id = ur.RoleId
                GROUP BY r.Id, r.Name
                ORDER BY r.Name";

            var roles = await connection.QueryAsync<RoleResponseDto>(sql);

            return roles.ToList();
        }
    }
}
