using Application.Common.Options;
using Application.DTOs.Review;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class ReviewRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), IReviewRepository
    {
        private string SelectColumns =>
            $@"r.id, r.headline, r.comment, r.rating, r.course_id, r.student_id, r.created_at, r.updated_at,
               CONCAT(u.first_name, ' ', u.last_name) AS student_name,
               CASE WHEN u.profile_picture_url IS NOT NULL 
                    THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) 
                    ELSE NULL END AS student_profile_picture";

        private const string FromClause =
            @"FROM reviews r
              JOIN students s ON r.student_id = s.id
              JOIN ""AspNetUsers"" u ON s.user_id = u.id";

        public Task<PaginatedResult<ReviewResponseDto>> GetByCourseAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<ReviewResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause} WHERE r.course_id = @CourseId",
                selectSql: $"SELECT {SelectColumns} {FromClause} WHERE r.course_id = @CourseId",
                allowedSortColumns: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "rating", "r.rating" },
                    { "created_at", "r.created_at" }
                },
                defaultSortColumn: "r.created_at",
                searchCondition: null,
                extraConditions: null,
                configureParameters: parameters =>
                {
                    parameters.Add("CourseId", courseId);
                },
                ct);
        }

        public async Task<ReviewResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {SelectColumns} {FromClause} WHERE r.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<ReviewResponseDto>(sql, new { Id = id });
        }

        public async Task<Review?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            return await connection.QueryFirstOrDefaultAsync<Review>(
                "SELECT * FROM reviews WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM enrollments WHERE student_id = @StudentId AND course_id = @CourseId",
                new { StudentId = studentId, CourseId = courseId });
            return count > 0;
        }

        public async Task<bool> HasStudentReviewedAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM reviews WHERE student_id = @StudentId AND course_id = @CourseId",
                new { StudentId = studentId, CourseId = courseId });
            return count > 0;
        }

        public async Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            return await connection.QueryFirstOrDefaultAsync<Guid?>(
                "SELECT id FROM students WHERE user_id = @UserId", new { UserId = userId });
        }

        public async Task<Guid> CreateAsync(Review review, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"INSERT INTO reviews (id, headline, comment, rating, course_id, student_id, created_at, updated_at)
                        VALUES (@Id, @Headline, @Comment, @Rating, @CourseId, @StudentId, @CreatedAt, @UpdatedAt);

                        UPDATE courses
                        SET total_reviews = total_reviews + 1,
                            average_rate  = (
                                SELECT ROUND(AVG(rating)::numeric, 1) FROM reviews WHERE course_id = @CourseId
                            )
                        WHERE id = @CourseId;";

            await connection.ExecuteAsync(sql, new 
            { 
                review.Id, 
                review.Headline, 
                review.Comment, 
                review.Rating, 
                review.CourseId, 
                review.StudentId,
                review.CreatedAt,
                review.UpdatedAt
            });
            return review.Id;
        }

        public async Task UpdateAsync(Review review, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"UPDATE reviews
                        SET headline    = @Headline,
                            comment     = @Comment,
                            rating      = @Rating,
                            updated_at  = @UpdatedAt
                        WHERE id = @Id;

                        UPDATE courses
                        SET average_rate = (
                            SELECT ROUND(AVG(rating)::numeric, 1) FROM reviews WHERE course_id = @CourseId
                        )
                        WHERE id = @CourseId;";

            await connection.ExecuteAsync(sql, new 
            { 
                review.Id, 
                review.Headline, 
                review.Comment, 
                review.Rating, 
                review.CourseId, 
                review.UpdatedAt
            });
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var review = await GetEntityByIdAsync(id, ct);
            if (review is null) return;

            var sql = @"DELETE FROM reviews WHERE id = @Id;

                        UPDATE courses
                        SET total_reviews = GREATEST(total_reviews - 1, 0),
                            average_rate  = COALESCE((
                                SELECT ROUND(AVG(rating)::numeric, 1) FROM reviews WHERE course_id = @CourseId
                            ), 0)
                        WHERE id = @CourseId;";

            await connection.ExecuteAsync(sql, new { Id = id, CourseId = review.CourseId });
        }
    }
}
