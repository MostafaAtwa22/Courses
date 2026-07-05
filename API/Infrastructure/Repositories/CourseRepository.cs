using Application.Common.Options;
using Application.DTOs.Course;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class CourseRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), ICourseRepository
    {
        private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            { "title", "c.title" },
            { "cost", "c.cost" },
            { "status", "c.status" },
            { "student_count", "c.student_count" },
            { "total_reviews", "c.total_reviews" },
            { "average_rate", "c.average_rate" },
            { "created_at", "c.created_at" },
            { "updated_at", "c.updated_at" }
        };

        private string SelectColumns =>
            $@"c.id, c.title, c.description, 
               CASE WHEN c.picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', c.picture_url) ELSE NULL END AS PictureUrl, 
               c.status, c.cost, 
               c.cost - c.cost * COALESCE((SELECT MAX(percentage) FROM course_discounts cd WHERE cd.course_id = c.id AND cd.is_active = true), 0) / 100.0 AS PriceAfterDiscount,
               c.student_count AS StudentCount, c.total_reviews AS TotalReviews, c.average_rate AS AverageRate, 
               c.language, c.what_you_will_learn AS WhatYouWillLearn, c.requirements AS Requirements,
               CASE WHEN c.intro_video_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', c.intro_video_url) ELSE NULL END AS IntroVideoUrl,
               cat.name AS Category,
               (SELECT CONCAT(u.first_name, ' ', u.last_name) 
                FROM instructors ins 
                JOIN ""AspNetUsers"" u ON ins.user_id = u.id 
                WHERE ins.id = c.instructor_id LIMIT 1) AS InstructorName,
               (SELECT CASE WHEN u.profile_picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) ELSE NULL END 
                FROM instructors ins 
                JOIN ""AspNetUsers"" u ON ins.user_id = u.id 
                WHERE ins.id = c.instructor_id LIMIT 1) AS InstructorProfilePicture,
               (SELECT ins.title 
                FROM instructors ins 
                WHERE ins.id = c.instructor_id LIMIT 1) AS InstructorTitle";

        private string SummaryColumns =>
            $@"c.id, c.title, c.cost,
               c.cost - c.cost * COALESCE((SELECT MAX(percentage) FROM course_discounts cd WHERE cd.course_id = c.id AND cd.is_active = true), 0) / 100.0 AS PriceAfterDiscount,
               c.total_reviews AS TotalReviews, c.average_rate AS AverageRate, c.language,
               CASE WHEN c.picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', c.picture_url) ELSE NULL END AS PictureUrl,
               cat.name AS Category,
               (SELECT CONCAT(u.first_name, ' ', u.last_name) 
                FROM instructors ins 
                JOIN ""AspNetUsers"" u ON ins.user_id = u.id 
                WHERE ins.id = c.instructor_id LIMIT 1) AS InstructorName";

        private const string FromClause =
            "FROM courses c JOIN categories cat ON c.category_id = cat.id";

        public Task<PaginatedResult<CourseSummaryDto>> GetAllAsync(CourseQueryParams queryParams, CancellationToken ct = default)
        {
            var extraConditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(queryParams.Category))
            {
                extraConditions.Add("cat.name ILIKE @Category");
            }

            if (queryParams.MinRating.HasValue)
            {
                extraConditions.Add("c.average_rate >= @MinRating");
            }
            if (queryParams.MaxRating.HasValue)
            {
                extraConditions.Add("c.average_rate <= @MaxRating");
            }

            return ExecutePaginatedQueryAsync<CourseSummaryDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause}",
                selectSql: $"SELECT {SummaryColumns} {FromClause}",
                allowedSortColumns: AllowedSortColumns,
                defaultSortColumn: "c.created_at",
                searchCondition: "(c.title ILIKE @SearchTerm OR c.description ILIKE @SearchTerm)",
                extraConditions: extraConditions,
                configureParameters: parameters =>
                {
                    if (!string.IsNullOrWhiteSpace(queryParams.Category))
                    {
                        parameters.Add("Category", queryParams.Category);
                    }
                    if (queryParams.MinRating.HasValue)
                    {
                        parameters.Add("MinRating", queryParams.MinRating.Value);
                    }
                    if (queryParams.MaxRating.HasValue)
                    {
                        parameters.Add("MaxRating", queryParams.MaxRating.Value);
                    }
                },
                ct);
        }

        public async Task<CourseResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $@"SELECT {SelectColumns} {FromClause} WHERE c.id = @Id;";
            return await connection.QueryFirstOrDefaultAsync<CourseResponseDto>(sql, new { Id = id });
        }

        public async Task<Course?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM courses WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Course>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(Course course, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"INSERT INTO courses (id, title, description, picture_url, status, cost, student_count, total_reviews, average_rate, language, what_you_will_learn, requirements, intro_video_url, category_id, instructor_id, created_at, updated_at)
                        VALUES (@Id, @Title, @Description, @PictureUrl, @Status, @Cost, @StudentCount, @TotalReviews, @AverageRate, @Language, @WhatYouWillLearn, @Requirements, @IntroVideoUrl, @CategoryId, @InstructorId, @CreatedAt, @UpdatedAt)";

            await connection.ExecuteAsync(sql, course);

            return course.Id;
        }

        public async Task UpdateAsync(Course course, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);

            var sql = @"UPDATE courses 
                        SET title = @Title, 
                            description = @Description, 
                            picture_url = @PictureUrl, 
                            status = @Status, 
                            cost = @Cost, 
                            student_count = @StudentCount,
                            total_reviews = @TotalReviews,
                            average_rate = @AverageRate,
                            language = @Language,
                            what_you_will_learn = @WhatYouWillLearn,
                            requirements = @Requirements,
                            intro_video_url = @IntroVideoUrl,
                            category_id = @CategoryId,
                            instructor_id = @InstructorId,
                            updated_at = @UpdatedAt
                        WHERE id = @Id";

            course.UpdatedAt = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, course);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"DELETE FROM courses WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<string>> GetSuggestionsAsync(string term, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"
                SELECT title FROM (
                    SELECT DISTINCT title FROM courses WHERE title ILIKE @Term
                    UNION
                    SELECT DISTINCT name FROM categories WHERE name ILIKE @Term
                ) AS suggestions
                LIMIT 10";
            
            return await connection.QueryAsync<string>(sql, new { Term = $"%{term}%" });
        }
    }
}