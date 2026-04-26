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

        public Task<PaginatedResult<CoursesResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<CoursesResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause}",
                selectSql: $"SELECT {SelectColumns} {FromClause}",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "c.created_at",
                searchCondition: "(c.title ILIKE @SearchTerm OR c.description ILIKE @SearchTerm)",
                ct);
        }

        public async Task<CoursesResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $@"SELECT {SelectColumns} {FromClause} WHERE c.id = @Id;";
            return await connection.QueryFirstOrDefaultAsync<CoursesResponseDto>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(CourseCreateDto courseCreateDto, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"INSERT INTO courses (id, title, description, picture_url, status, cost, category_id)
                        VALUES (@id, @Title, @Description, @PictureUrl, @Status, @Cost, @CategoryId)";

            var id = Guid.NewGuid();
            await connection.ExecuteAsync(sql, new {
                id,
                courseCreateDto.Title,
                courseCreateDto.Description,
                courseCreateDto.PictureUrl,
                courseCreateDto.Status,
                courseCreateDto.Cost,
                courseCreateDto.CategoryId
            });

            return id;
        }

        public async Task UpdateAsync(Guid id, CourseUpdateDto courseUpdateDto, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            
            var sql = @"UPDATE courses SET title = @Title, description = @Description, picture_url = @PictureUrl, status = @Status, cost = @Cost, category_id = @CategoryId
                        WHERE id = @Id";

            await connection.ExecuteAsync(sql, new {
                Id = id,
                courseUpdateDto.Title,
                courseUpdateDto.Description,
                courseUpdateDto.PictureUrl,
                courseUpdateDto.Status,
                courseUpdateDto.Cost,
                courseUpdateDto.CategoryId
            });
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM courses WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}