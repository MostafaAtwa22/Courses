using Application.Common.Interfaces;
using Application.DTOs.InstructorDashboard;

namespace Infrastructure.Repositories
{
    public class InstructorDashboardRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), IInstructorDashboardRepository
    {
        public async Task<InstructorStatisticsDto?> GetInstructorStatisticsAsync(Guid instructorId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                WITH current_month AS (
                    SELECT 
                        COUNT(DISTINCT e.student_id) AS student_count,
                        COALESCE(SUM(c.cost), 0) AS earnings
                    FROM enrollments e 
                    JOIN courses c ON e.course_id = c.id 
                    WHERE c.instructor_id = @InstructorId 
                    AND EXTRACT(MONTH FROM e.created_at) = EXTRACT(MONTH FROM NOW()) 
                    AND EXTRACT(YEAR FROM e.created_at) = EXTRACT(YEAR FROM NOW())
                ),
                previous_month AS (
                    SELECT 
                        COUNT(DISTINCT e.student_id) AS student_count,
                        COALESCE(SUM(c.cost), 0) AS earnings
                    FROM enrollments e 
                    JOIN courses c ON e.course_id = c.id 
                    WHERE c.instructor_id = @InstructorId 
                    AND EXTRACT(MONTH FROM e.created_at) = EXTRACT(MONTH FROM NOW() - INTERVAL '1 month') 
                    AND EXTRACT(YEAR FROM e.created_at) = EXTRACT(YEAR FROM NOW() - INTERVAL '1 month')
                ),
                current_courses AS (
                    SELECT COUNT(*) AS course_count
                    FROM courses 
                    WHERE instructor_id = @InstructorId
                ),
                previous_courses AS (
                    SELECT COUNT(*) AS course_count
                    FROM courses 
                    WHERE instructor_id = @InstructorId
                    AND created_at < DATE_TRUNC('month', NOW())
                ),
                current_rating AS (
                    SELECT COALESCE(AVG(r.rating), 0) AS avg_rating
                    FROM reviews r 
                    JOIN courses c ON r.course_id = c.id 
                    WHERE c.instructor_id = @InstructorId
                    AND r.created_at >= DATE_TRUNC('month', NOW())
                ),
                previous_rating AS (
                    SELECT COALESCE(AVG(r.rating), 0) AS avg_rating
                    FROM reviews r 
                    JOIN courses c ON r.course_id = c.id 
                    WHERE c.instructor_id = @InstructorId
                    AND r.created_at >= DATE_TRUNC('month', NOW() - INTERVAL '1 month')
                    AND r.created_at < DATE_TRUNC('month', NOW())
                )
                SELECT 
                    (SELECT COUNT(DISTINCT e.student_id) 
                     FROM enrollments e 
                     JOIN courses c ON e.course_id = c.id 
                     WHERE c.instructor_id = @InstructorId) AS EnrolledStudents,
                    
                    CASE 
                        WHEN (SELECT student_count FROM previous_month) = 0 THEN 0
                        ELSE ROUND(((SELECT student_count FROM current_month) - (SELECT student_count FROM previous_month))::numeric / 
                                   NULLIF((SELECT student_count FROM previous_month), 0) * 100, 2)
                    END AS EnrolledStudentsChange,
                    
                    (SELECT COALESCE(SUM(c.cost), 0) 
                     FROM enrollments e 
                     JOIN courses c ON e.course_id = c.id 
                     WHERE c.instructor_id = @InstructorId 
                     AND EXTRACT(MONTH FROM e.created_at) = EXTRACT(MONTH FROM NOW()) 
                     AND EXTRACT(YEAR FROM e.created_at) = EXTRACT(YEAR FROM NOW())) AS MonthlyEarnings,
                    
                    CASE 
                        WHEN (SELECT earnings FROM previous_month) = 0 THEN 0
                        ELSE ROUND(((SELECT earnings FROM current_month) - (SELECT earnings FROM previous_month))::numeric / 
                                   NULLIF((SELECT earnings FROM previous_month), 0) * 100, 2)
                    END AS MonthlyEarningsChange,
                    
                    (SELECT COALESCE(AVG(r.rating), 0) 
                     FROM reviews r 
                     JOIN courses c ON r.course_id = c.id 
                     WHERE c.instructor_id = @InstructorId) AS InstructorRating,
                    
                    CASE 
                        WHEN (SELECT avg_rating FROM previous_rating) = 0 THEN 0
                        ELSE ROUND(((SELECT avg_rating FROM current_rating) - (SELECT avg_rating FROM previous_rating))::numeric, 2)
                    END AS InstructorRatingChange,
                    
                    (SELECT COUNT(*) 
                     FROM courses 
                     WHERE instructor_id = @InstructorId) AS CoursesCreated,
                    
                    CASE 
                        WHEN (SELECT course_count FROM previous_courses) = 0 THEN 0
                        ELSE ROUND(((SELECT course_count FROM current_courses) - (SELECT course_count FROM previous_courses))::numeric / 
                                   NULLIF((SELECT course_count FROM previous_courses), 0) * 100, 2)
                    END AS CoursesCreatedChange";

            return await connection.QueryFirstOrDefaultAsync<InstructorStatisticsDto>(sql, new { InstructorId = instructorId });
        }
    }
}