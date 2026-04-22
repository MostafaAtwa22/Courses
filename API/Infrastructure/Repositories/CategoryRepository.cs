using System.Data;
using Application.Common.Interfaces;
using Application.DTOs.Category;
using Dapper;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(IDbConnectionFactory _factory) : ICategoryRepository
    {
        private async Task<IDbConnection> CreateConnectionAsync(CancellationToken ct) =>
            await _factory.CreateConnectionAsync(ct);

        public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT Id, Name, Slug, CreatedAt, UpdatedAt
                        FROM Categories";
            return (await connection.QueryAsync<CategoryResponseDto>(sql)).AsList();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT Id, Name, Slug, CreatedAt, UpdatedAt
                        FROM Categories
                        WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<CategoryResponseDto>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"INSERT INTO Categories (Id, Name, Slug, CreatedAt, UpdatedAt)
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
            var sql = @"UPDATE Categories
                        SET Name = @Name, Slug = @Slug, UpdatedAt = @UpdatedAt
                        WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id, dto.Name, dto.Slug, UpdatedAt = DateTime.UtcNow });
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM Categories WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}