using Application.DTOs.Category;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), ICategoryRepository
    {
        private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "name", "name" },
            { "slug", "slug" },
            { "created_at", "created_at" },
            { "updated_at", "updated_at" }
        };

        public Task<PaginatedResult<CategoryResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<CategoryResponseDto>(
                queryParams,
                countSql: "SELECT COUNT(1) FROM categories",
                selectSql: "SELECT id, name, slug, created_at, updated_at FROM categories",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "created_at",
                searchCondition: "(name ILIKE @SearchTerm OR slug ILIKE @SearchTerm)",
                ct);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT id, name, slug, created_at, updated_at
                        FROM categories
                        WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<CategoryResponseDto>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"INSERT INTO categories (id, name, slug, created_at, updated_at)
                        VALUES (@Id, @Name, @Slug, @CreatedAt, @UpdatedAt)";
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            await connection.ExecuteAsync(sql,
                new { Id = id, dto.Name, dto.Slug, CreatedAt = now, UpdatedAt = now });
            return id;
        }

        public async Task UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"UPDATE categories
                        SET name = @Name, slug = @Slug, updated_at = @UpdatedAt
                        WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, dto.Name, dto.Slug, UpdatedAt = DateTime.UtcNow });
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM categories WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}