using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Admin;
using Dapper;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class AdminRepository(
    IDbConnectionFactory factory,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<UrlsOptions> urlsOptions)
    : BaseRepository(factory), IAdminRepository
{
    private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "name", "first_name" },
        { "firstName", "first_name" },
        { "lastName", "last_name" },
        { "email", "email" },
        { "userName", "user_name" },
        { "gender", "gender" },
        { "role", "role" }
    };

    private string SelectColumns =>
        $@"CAST(u.id AS uuid) AS Id,
           u.first_name AS FirstName,
           u.last_name AS LastName,
           u.email AS Email,
           u.user_name AS UserName,
           u.gender AS Gender,
           CASE WHEN u.profile_picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) ELSE NULL END AS ProfilePicture,
           r.name AS Role";

    private const string FromClause = @"FROM ""AspNetUsers"" u
                                       JOIN ""AspNetUserRoles"" ur ON u.id = ur.user_id
                                       JOIN ""AspNetRoles"" r ON ur.role_id = r.id";

    public async Task<AdminResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = $"SELECT {SelectColumns} {FromClause} WHERE r.name IN ('Admin', 'SuperAdmin') AND u.id = @Id";
        return await connection.QueryFirstOrDefaultAsync<AdminResponseDto>(sql, new { Id = id.ToString() });
    }

    public async Task<AdminResponseDto?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = $"SELECT {SelectColumns} {FromClause} WHERE r.name IN ('Admin', 'SuperAdmin') AND u.id = @UserId";
        return await connection.QueryFirstOrDefaultAsync<AdminResponseDto>(sql, new { UserId = userId });
    }

    public async Task<PaginatedResult<AdminResponseDto>> GetAllAsync(AdminQueryParams queryParams, CancellationToken ct = default)
    {
        var extraConditions = new List<string>
        {
            // Base condition: only get Admin and SuperAdmin roles
            "r.name IN ('Admin', 'SuperAdmin')"
        };

        if (queryParams.Role.HasValue)
        {
            extraConditions.Add("r.name = @Role");
        }

        if (queryParams.Gender.HasValue)
        {
            extraConditions.Add("u.gender = @Gender");
        }

        return await ExecutePaginatedQueryAsync<AdminResponseDto>(
            queryParams,
            countSql: $"SELECT COUNT(1) {FromClause}",
            selectSql: $"SELECT {SelectColumns} {FromClause}",
            allowedSortColumns: AllowedSortColumns,
            defaultSortColumn: "first_name",
            searchCondition: "(u.first_name ILIKE @SearchTerm OR u.last_name ILIKE @SearchTerm OR u.email ILIKE @SearchTerm OR u.user_name ILIKE @SearchTerm)",
            extraConditions: extraConditions,
            configureParameters: parameters =>
            {
                if (queryParams.Role.HasValue)
                {
                    parameters.Add("Role", queryParams.Role.Value.ToString());
                }
                if (queryParams.Gender.HasValue)
                {
                    parameters.Add("Gender", queryParams.Gender.Value.ToString());
                }
            },
            ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await userManager.DeleteAsync(user);
        }
    }

    public async Task<bool> IsLastSuperAdminAsync(CancellationToken ct = default)
    {
        var superAdminRole = await roleManager.FindByNameAsync(Role.SuperAdmin.ToString());
        if (superAdminRole == null) return false;

        var superAdmins = await userManager.GetUsersInRoleAsync(Role.SuperAdmin.ToString());
        return superAdmins.Count <= 1;
    }
}