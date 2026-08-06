namespace Infrastructure.Repositories
{
    public class ContentProgressRepository(IDbConnectionFactory factory) 
        : BaseRepository(factory), IContentProgressRepository
    {
        public async Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT id 
                FROM students
                WHERE user_id = @UserId";
            
            return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { UserId = userId });
        }

        public async Task MarkCompleteAsync(Guid studentId, Guid contentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                INSERT INTO content_progress (id, student_id, content_id, course_id, completed_at, created_at, updated_at)
                VALUES (@Id, @StudentId, @ContentId, @CourseId, @CompletedAt, @CreatedAt, @UpdatedAt)
                ON CONFLICT (student_id, content_id) 
                DO UPDATE SET 
                    completed_at = @CompletedAt,
                    updated_at = @UpdatedAt";
            
            await connection.ExecuteAsync(sql, new 
            { 
                Id = Guid.NewGuid(),
                StudentId = studentId, 
                ContentId = contentId, 
                CourseId = courseId,
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task MarkIncompleteAsync(Guid studentId, Guid contentId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                DELETE FROM content_progress 
                WHERE student_id = @StudentId AND content_id = @ContentId";
            
            await connection.ExecuteAsync(sql, new { StudentId = studentId, ContentId = contentId });
        }

        public async Task<HashSet<Guid>> GetCompletedContentIdsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT content_id 
                FROM content_progress
                WHERE student_id = @StudentId AND course_id = @CourseId";
            
            var contentIds = await connection.QueryAsync<Guid>(sql, new { StudentId = studentId, CourseId = courseId });
            return contentIds.ToHashSet();
        }

        public async Task<CourseProgressSummary> GetCourseProgressSummaryAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                WITH course_contents AS (
                    SELECT c.id
                    FROM contents c
                    JOIN sections s ON c.section_id = s.id
                    WHERE s.course_id = @CourseId
                ),
                completed_contents AS (
                    SELECT COUNT(*)::int as completed_count
                    FROM content_progress cp
                    WHERE cp.student_id = @StudentId 
                      AND cp.course_id = @CourseId
                ),
                total_contents AS (
                    SELECT COUNT(*)::int as total_count
                    FROM course_contents
                )
                SELECT 
                    @CourseId as course_id,
                    COALESCE(cc.completed_count, 0) as completed_count,
                    tc.total_count,
                    CASE 
                        WHEN tc.total_count = 0 THEN 0
                        ELSE ROUND((COALESCE(cc.completed_count, 0)::decimal / tc.total_count) * 100)::int
                    END as percent_complete
                FROM total_contents tc
                CROSS JOIN completed_contents cc";
            
            return await connection.QueryFirstOrDefaultAsync<CourseProgressSummary>(sql, new { StudentId = studentId, CourseId = courseId })
                ?? new CourseProgressSummary { CourseId = courseId, CompletedCount = 0, TotalCount = 0, PercentComplete = 0 };
        }

        public async Task<IReadOnlyList<CourseProgressSummary>> GetMyCoursesProgressAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                WITH enrolled_courses AS (
                    SELECT DISTINCT e.course_id
                    FROM enrollments e
                    WHERE e.student_id = @StudentId
                ),
                course_totals AS (
                    SELECT 
                        s.course_id,
                        COUNT(c.id)::int as total_count
                    FROM sections s
                    JOIN contents c ON c.section_id = s.id
                    WHERE s.course_id IN (SELECT course_id FROM enrolled_courses)
                    GROUP BY s.course_id
                ),
                course_completed AS (
                    SELECT 
                        cp.course_id,
                        COUNT(cp.id)::int as completed_count
                    FROM content_progress cp
                    WHERE cp.student_id = @StudentId
                      AND cp.course_id IN (SELECT course_id FROM enrolled_courses)
                    GROUP BY cp.course_id
                )
                SELECT 
                    ct.course_id,
                    COALESCE(cc.completed_count, 0) as completed_count,
                    ct.total_count,
                    CASE 
                        WHEN ct.total_count = 0 THEN 0
                        ELSE ROUND((COALESCE(cc.completed_count, 0)::decimal / ct.total_count) * 100)::int
                    END as percent_complete
                FROM course_totals ct
                LEFT JOIN course_completed cc ON ct.course_id = cc.course_id
                ORDER BY ct.course_id";
            
            var results = await connection.QueryAsync<CourseProgressSummary>(sql, new { StudentId = studentId });
            return results.ToList();
        }
    }
}
