using System.Data;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Category;
using Dapper;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(IDbConnectionFactory _factory) : ICategoryRepository
    {
        private async Task<IDbConnection> CreateConnectionAsync(CancellationToken ct) =>
            await _factory.CreateConnectionAsync(ct);

        public async Task<PaginatedResult<CategoryResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var parameters = new DynamicParameters();
            var conditions = new List<string>();

            int pageNumber = queryParams.PageNumber ?? 1;
            int pageSize = queryParams.PageSize ?? 10;
            if (pageSize > 50) pageSize = 50;

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                conditions.Add("(name ILIKE @SearchTerm OR slug ILIKE @SearchTerm)");
                parameters.Add("SearchTerm", $"%{queryParams.SearchTerm}%");
            }

            var whereClause = conditions.Count != 0
                ? $"WHERE {string.Join(" AND ", conditions)}"
                : string.Empty;

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "name" },
                { "slug", "slug" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" }
            };

            var orderByColumn = !string.IsNullOrWhiteSpace(queryParams.SortBy) && allowedSortColumns.TryGetValue(queryParams.SortBy, out var dbColumn)
                ? dbColumn
                : "created_at";

            var sortDirection = (queryParams.SortDescending ?? false) ? "DESC" : "ASC";

            var sql = $@"
                SELECT COUNT(1) 
                FROM categories 
                {whereClause};

                SELECT id, name, slug, created_at, updated_at
                FROM categories
                {whereClause}
                ORDER BY {orderByColumn} {sortDirection}
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<CategoryResponseDto>()).AsList();

            return new PaginatedResult<CategoryResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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