using Application.Common.Interfaces;
using Application.DTOs.AdminDashboard;

namespace Infrastructure.Repositories
{
    public class AdminDashboardRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), IAdminDashboardRepository
    {
        public async Task<IEnumerable<EnrollmentStatisticsDto>> GetEnrollmentStatisticsAsync(CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                WITH all_months AS (
                    SELECT 
                        generate_series(1, 12) AS month_num,
                        TO_CHAR(DATE_TRUNC('month', NOW() - INTERVAL '11 months') + (generate_series(1, 12) - 1) * INTERVAL '1 month', 'Mon') AS month_name
                ),
                enrollment_counts AS (
                    SELECT 
                        EXTRACT(MONTH FROM created_at) AS month_num,
                        COUNT(*) AS EnrollmentCount
                    FROM enrollments
                    WHERE created_at >= NOW() - INTERVAL '12 months'
                    GROUP BY EXTRACT(MONTH FROM created_at)
                )
                SELECT 
                    am.month_name AS Period,
                    COALESCE(ec.EnrollmentCount, 0) AS EnrollmentCount
                FROM all_months am
                LEFT JOIN enrollment_counts ec ON am.month_num = ec.month_num
                ORDER BY am.month_num";

            return await connection.QueryAsync<EnrollmentStatisticsDto>(sql);
        }
    }
}
