using Application.Common.Interfaces;
using Application.DTOs.Course;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CourseDiscountRepository(IDbConnectionFactory factory) : BaseRepository(factory), ICourseDiscountRepository
    {
        public async Task<IEnumerable<CourseDiscountDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"SELECT id, percentage, start_time AS StartTime, end_time AS EndTime, is_active AS IsActive, 
                               course_id AS CourseId, created_at AS CreatedAt, updated_at AS UpdatedAt 
                        FROM course_discounts 
                        WHERE course_id = @CourseId 
                        ORDER BY created_at DESC";
            
            return await connection.QueryAsync<CourseDiscountDto>(sql, new { CourseId = courseId });
        }

        public async Task<Guid> AddAsync(CourseDiscount discount, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            
            var sql = @"INSERT INTO course_discounts (id, percentage, start_time, end_time, is_active, course_id, created_at, updated_at)
                        VALUES (@Id, @Percentage, @StartTime, @EndTime, @IsActive, @CourseId, @CreatedAt, @UpdatedAt)
                        RETURNING id";

            return await connection.QuerySingleAsync<Guid>(sql, discount);
        }

        public async Task<CourseDiscount?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM course_discounts WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<CourseDiscount>(sql, new { Id = id });
        }

        public async Task UpdateAsync(CourseDiscount discount, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"UPDATE course_discounts 
                        SET percentage = @Percentage, 
                            start_time = @StartTime, 
                            end_time = @EndTime, 
                            is_active = @IsActive,
                            updated_at = @UpdatedAt
                        WHERE id = @Id";
            discount.UpdatedAt = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, discount);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM course_discounts WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task DeactivateExpiredAsync(CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            
            var sql = @"UPDATE course_discounts 
                        SET is_active = false, updated_at = @UpdatedAt 
                        WHERE end_time <= CURRENT_TIMESTAMP 
                        AND is_active = true";

            await connection.ExecuteAsync(sql, new { UpdatedAt = DateTime.UtcNow });
        }
    }
}
