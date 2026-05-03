using Application.DTOs.Section;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SectionRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), ISectionRepository
    {
        private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "title", "title" },
            { "order", "\"order\"" },
            { "created_at", "created_at" },
            { "updated_at", "updated_at" }
        };

        public async Task<PaginatedResult<SectionResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            return await ExecutePaginatedQueryAsync<SectionResponseDto>(
                queryParams,
                countSql: "SELECT COUNT(1) FROM sections",
                selectSql: "SELECT id, title, \"order\", created_at, updated_at FROM sections",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "\"order\"",
                searchCondition: "title ILIKE @SearchTerm",
                extraConditions: null,
                configureParameters: null,
                ct: ct
            );
        }

        public async Task<PaginatedResult<SectionResponseDto>> GetByCourseIdAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default)
        {
            return await ExecutePaginatedQueryAsync<SectionResponseDto>(
                queryParams,
                countSql: "SELECT COUNT(1) FROM sections",
                selectSql: "SELECT id, title, \"order\", created_at, updated_at FROM sections",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "\"order\"",
                searchCondition: "title ILIKE @SearchTerm",
                extraConditions: ["course_id = @CourseId"],
                configureParameters: parameters => parameters.Add("CourseId", courseId),
                ct: ct
            );
        }

        public async Task<SectionResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT id, title, ""order"", created_at, updated_at 
                        FROM sections 
                        WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<SectionResponseDto>(sql, new { Id = id });
        }

        public async Task<Section?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT id, title, ""order"", course_id, created_at, updated_at 
                        FROM sections 
                        WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Section>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(Section section, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                INSERT INTO sections (id, title, ""order"", course_id, created_at, updated_at)
                VALUES (@Id, @Title, @Order, @CourseId, @CreatedAt, @UpdatedAt);";

            await connection.ExecuteAsync(sql, section);
            return section.Id;
        }

        public async Task UpdateAsync(Section section, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                UPDATE sections
                SET title = @Title,
                    ""order"" = @Order,
                    updated_at = @UpdatedAt
                WHERE id = @Id;";
            
            section.UpdatedAt = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, section);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "DELETE FROM sections WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}