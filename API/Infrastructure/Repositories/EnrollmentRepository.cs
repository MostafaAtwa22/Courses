namespace Infrastructure.Repositories
{
    public class EnrollmentRepository(IDbConnectionFactory factory) 
        : BaseRepository(factory), IEnrollmentRepository
    {
        public async Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT COUNT(1) 
                FROM enrollments e
                JOIN students s ON e.student_id = s.id
                WHERE s.id = @StudentId AND e.course_id = @CourseId";
            
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { StudentId = studentId, CourseId = courseId });
            return count > 0;
        }

        public async Task<bool> IsEnrolledByUserIdAsync(string userId, Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT COUNT(1) 
                FROM enrollments e
                JOIN students s ON e.student_id = s.id
                WHERE s.user_id = @UserId AND e.course_id = @CourseId";
            
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { UserId = userId, CourseId = courseId });
            return count > 0;
        }

        public async Task<Guid?> GetCourseIdByContentIdAsync(Guid contentId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT s.course_id 
                FROM contents c
                JOIN sections s ON c.section_id = s.id
                WHERE c.id = @ContentId";
            
            return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { ContentId = contentId });
        }

        public async Task<Guid?> GetInstructorIdByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            using var connection = await CreateConnectionAsync(cancellationToken);
            var sql = @"
                SELECT instructor_id 
                FROM courses 
                WHERE id = @CourseId";
            
            return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { CourseId = courseId });
        }
    }
}
