using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.Common.Options;
using Application.DTOs.Instructor;
using Dapper;
using Domain.Entities.Identity;
using Domain.Enums;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class InstructorRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), IInstructorRepository
    {
        private string SelectColumns =>
            $@"i.id, i.bio, i.title, 
               i.linked_in_profile_url AS LinkedInProfileUrl, 
               i.git_hub_profile_url AS GitHubProfileUrl, 
               i.status,
               (SELECT CONCAT(u.first_name, ' ', u.last_name) 
                FROM ""AspNetUsers"" u 
                WHERE u.id = i.user_id LIMIT 1) AS FullName,
               (SELECT CASE WHEN u.profile_picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) ELSE NULL END 
                FROM ""AspNetUsers"" u 
                WHERE u.id = i.user_id LIMIT 1) AS ProfilePictureUrl";

        private string PrivateSelectColumns =>
            $@"{SelectColumns}, 
               CASE WHEN i.cv_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', i.cv_url) ELSE NULL END AS CvUrl";

        private const string FromClause = "FROM instructors i";

        public async Task<Instructor?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM instructors WHERE user_id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Instructor>(sql, new { UserId = userId });
        }
        
        public async Task<Instructor?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM instructors WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Instructor>(sql, new { Id = id });
        }
        
        public async Task<InstructorPublicResponseDto?> GetPublicByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {SelectColumns} {FromClause} WHERE i.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<InstructorPublicResponseDto>(sql, new { Id = id });
        }

        public async Task<InstructorPrivateResponseDto?> GetPrivateByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {PrivateSelectColumns} {FromClause} WHERE i.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<InstructorPrivateResponseDto>(sql, new { Id = id });
        }
    
        public async Task<Guid> CreateAsync(Instructor instructor, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"INSERT INTO instructors (id, bio, title, linked_in_profile_url, git_hub_profile_url, cv_url, status, user_id, created_at, updated_at)
                        VALUES (@Id, @Bio, @Title, @LinkedInProfileUrl, @GitHubProfileUrl, @CvUrl, @Status, @UserId, @CreatedAt, @UpdatedAt)";

            await connection.ExecuteAsync(sql, instructor);

            return instructor.Id;
        }

        public async Task UpdateAsync(Instructor instructor, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"UPDATE instructors 
                        SET bio = @Bio, 
                            title = @Title, 
                            linked_in_profile_url = @LinkedInProfileUrl, 
                            git_hub_profile_url = @GitHubProfileUrl, 
                            cv_url = @CvUrl, 
                            status = @Status, 
                            user_id = @UserId, 
                            updated_at = @UpdatedAt
                        WHERE id = @Id";

            instructor.UpdatedAt = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, instructor);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM instructors WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<PaginatedResult<InstructorPrivateResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            
            var baseSql = $"{PrivateSelectColumns} {FromClause}";
            var countSql = $"SELECT COUNT(*) {FromClause}";
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                whereClause += " AND (i.bio ILIKE @Search OR i.title ILIKE @Search)";
                parameters.Add("Search", $"%{queryParams.Search}%");
            }
            
            var finalSql = $"{baseSql} {whereClause} ORDER BY i.created_at DESC LIMIT @PageSize OFFSET @Offset";
            var finalCountSql = $"{countSql} {whereClause}";
            
            parameters.Add("PageSize", queryParams.PageSize);
            parameters.Add("Offset", (queryParams.PageNumber - 1) * queryParams.PageSize);
            
            var items = await connection.QueryAsync<InstructorPrivateResponseDto>(finalSql, parameters);
            var totalCount = await connection.QuerySingleAsync<int>(finalCountSql, parameters);
            
            return PaginatedResult<InstructorPrivateResponseDto>.Success(
                items.ToList(), 
                totalCount, 
                queryParams.PageNumber, 
                queryParams.PageSize);
        }

        public async Task UpdateStatusAsync(Guid id, InstructorStatus status, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"UPDATE instructors SET status = @Status, updated_at = @UpdatedAt WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, Status = status, UpdatedAt = DateTime.UtcNow });
        }
    }
}
