using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Infrastructure.Repositories
{
    public class InstructorRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), IInstructorRepository
    {
        public async Task<Instructor?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM instructors WHERE user_id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Instructor>(sql, new { UserId = userId });
        }

        public async Task<Instructor?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM instructors WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Instructor>(sql, new { Id = id });
        }
    }
}
