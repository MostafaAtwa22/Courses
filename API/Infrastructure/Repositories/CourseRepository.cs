using Application.Common.Options;
using Application.DTOs.Course;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class CourseRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), ICourseRepository
    {
        private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "title", "c.title" },
            { "cost", "c.cost" },
            { "status", "c.status" },
            { "created_at", "c.created_at" },
            { "updated_at", "c.updated_at" }
        };

        private string SelectColumns =>
            $"c.id, c.title, c.description, CASE WHEN c.picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}', c.picture_url) ELSE NULL END AS picture_url, c.status, c.cost, cat.name AS category";

        private const string FromClause =
            "FROM courses c JOIN categories cat ON c.category_id = cat.id";

        public Task<PaginatedResult<CourseResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<CourseResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause}",
                selectSql: $"SELECT {SelectColumns} {FromClause}",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "c.created_at",
                searchCondition: "(c.title ILIKE @SearchTerm OR c.description ILIKE @SearchTerm)",
                ct);
        }

        public async Task<CourseResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $@"SELECT {SelectColumns} {FromClause} WHERE c.id = @Id;";
            return await connection.QueryFirstOrDefaultAsync<CourseResponseDto>(sql, new { Id = id });
        }

        public async Task<Course?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM courses WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Course>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(Course course, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"INSERT INTO courses (id, title, description, picture_url, status, cost, category_id, created_at, updated_at)
                        VALUES (@Id, @Title, @Description, @PictureUrl, @Status, @Cost, @CategoryId, @CreatedAt, @UpdatedAt)";

            await connection.ExecuteAsync(sql, course);

            return course.Id;
        }

        public async Task UpdateAsync(Course course, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"UPDATE courses 
                        SET title = @Title, 
                            description = @Description, 
                            picture_url = @PictureUrl, 
                            status = @Status, 
                            cost = @Cost, 
                            category_id = @CategoryId,
                            updated_at = @UpdatedAt
                        WHERE id = @Id";

            course.UpdatedAt = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, course);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM courses WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}