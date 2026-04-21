using Application.Common.Interfaces;
using Application.DTOs.Category;
using Dapper;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(IDbConnectionFactory _factory) : ICategoryRepository
    {
        public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var connection = await _factory.CreateConnectionAsync(ct);

            var sql = @"SELECT Id, Name, Description
                        FROM Categories";
            
            return (await connection.QueryAsync<CategoryResponseDto>(sql)).AsList();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var connection = await _factory.CreateConnectionAsync(ct);
            var sql = @"SELECT Id, Name, Description
                        FROM Categories
                        WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<CategoryResponseDto>(sql, new { Id = id });
        }
    }
}