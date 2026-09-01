using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Instructor;
using Domain.Enums;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class InstructorRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), IInstructorRepository
    {
        private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "name", "FullName" },
            { "average_rate", "AverageRate" },
            { "created_at", "i.created_at" }
        };

        private string SelectColumns =>
            $@"i.id, i.bio, i.title, 
               i.linked_in_profile_url AS LinkedInProfileUrl, 
               i.git_hub_profile_url AS GitHubProfileUrl, 
               i.status,
               i.created_at AS CreatedAt,
               i.updated_at AS UpdatedAt,
               u.first_name AS FirstName,
               u.last_name AS LastName,
               u.email AS Email,
               u.user_name AS UserName,
               u.gender AS Gender,
               CASE WHEN u.profile_picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) ELSE NULL END AS ProfilePicture,
               (SELECT COALESCE(AVG(c.average_rate), 0) FROM courses c WHERE c.instructor_id = i.id) AS AverageRate,
               (SELECT COUNT(*) FROM reviews cr 
                JOIN courses c ON cr.course_id = c.id 
                WHERE c.instructor_id = i.id) AS TotalReviews,
               (SELECT COUNT(DISTINCT e.student_id) FROM enrollments e 
                JOIN courses c ON e.course_id = c.id 
                WHERE c.instructor_id = i.id) AS TotalStudents,
               (SELECT COUNT(*) FROM courses c WHERE c.instructor_id = i.id) AS TotalCourses";

        private string PrivateSelectColumns =>
            $@"{SelectColumns}, 
               CASE WHEN i.cv_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', i.cv_url) ELSE NULL END AS CvUrl";

        private const string FromClause = "FROM instructors i JOIN \"AspNetUsers\" u ON i.user_id = u.id";

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

        public async Task<InstructorPublicResponseDto?> GetPublicByCourseIdAsync(
            Guid courseId,
            CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = $@"
                SELECT {SelectColumns}
                {FromClause}
                JOIN courses c ON i.id = c.instructor_id
                WHERE c.id = @CourseId";

            return await connection.QueryFirstOrDefaultAsync<InstructorPublicResponseDto>(
                sql,
                new { CourseId = courseId });
        }

        public async Task<InstructorPrivateResponseDto?> GetPrivateByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {PrivateSelectColumns} {FromClause} WHERE i.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<InstructorPrivateResponseDto>(sql, new { Id = id });
        }

        public async Task<InstructorPrivateResponseDto?> GetPrivateByUserIdAsync(string userId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {PrivateSelectColumns} {FromClause} WHERE i.user_id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<InstructorPrivateResponseDto>(sql, new { UserId = userId });
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

        public Task<PaginatedResult<InstructorPrivateResponseDto>> GetAllAsync(InstructorQueryParams queryParams, CancellationToken ct = default)
        {
            var extraConditions = new List<string>();

            if (queryParams.Status.HasValue)
            {
                extraConditions.Add("i.status = @Status");
            }

            return ExecutePaginatedQueryAsync<InstructorPrivateResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause}",
                selectSql: $"SELECT {PrivateSelectColumns} {FromClause}",
                allowedSortColumns: AllowedSortColumns, 
                defaultSortColumn: "i.created_at",
                searchCondition: "(i.bio ILIKE @SearchTerm OR i.title ILIKE @SearchTerm)",
                extraConditions: extraConditions,
                configureParameters: parameters =>
                {
                    if (queryParams.Status.HasValue)
                    {
                        parameters.Add("Status", queryParams.Status.Value);
                    }
                },
                ct);
        }

        public async Task UpdateStatusAsync(Guid id, InstructorStatus status, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"UPDATE instructors SET status = @Status, updated_at = @UpdatedAt WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, Status = status, UpdatedAt = DateTime.UtcNow });
        }

        public async Task<Guid?> GetInstructorIdByUserIdAsync(string userId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                SELECT id 
                FROM instructors
                WHERE user_id = @UserId";
            
            return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { UserId = userId });
        }
    }
}
