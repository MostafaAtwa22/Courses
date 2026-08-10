using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Student;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class StudentRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
    : BaseRepository(factory), IStudentRepository
{
    private static readonly Dictionary<string, string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "name", "FullName" },
        { "total_enrollments", "TotalEnrollments" },
        { "created_at", "s.created_at" }
    };

    private string SelectColumns =>
        $@"s.id, 
           s.created_at AS CreatedAt,
           s.updated_at AS UpdatedAt,
           u.first_name AS FirstName,
           u.last_name AS LastName,
           u.email AS Email,
           u.user_name AS UserName,
           u.gender AS Gender,
           CASE WHEN u.profile_picture_url IS NOT NULL THEN CONCAT('{urlsOptions.Value.API}/', u.profile_picture_url) ELSE NULL END AS ProfilePicture,
           (SELECT COUNT(*) FROM enrollments e WHERE e.student_id = s.id) AS TotalEnrollments";

    private const string FromClause = "FROM students s JOIN \"AspNetUsers\" u ON s.user_id = u.id";

    public async Task<Student?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = "SELECT * FROM students WHERE user_id = @UserId";
        return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { UserId = userId });
    }

    public async Task<Student?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = "SELECT * FROM students WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { Id = id });
    }

    public async Task<StudentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = $"SELECT {SelectColumns} {FromClause} WHERE s.id = @Id";
        return await connection.QueryFirstOrDefaultAsync<StudentResponseDto>(sql, new { Id = id });
    }

    public async Task<Guid> CreateAsync(Student student, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);

        var sql = @"INSERT INTO students (id, user_id, created_at, updated_at)
                    VALUES (@Id, @UserId, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(sql, student);

        return student.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = @"DELETE FROM students WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var connection = await CreateConnectionAsync(ct);
        var sql = @"
            SELECT id 
            FROM students
            WHERE user_id = @UserId";
        
        return await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { UserId = userId });
    }

    public Task<PaginatedResult<StudentResponseDto>> GetAllAsync(StudentQueryParams queryParams, CancellationToken ct = default)
    {
        var extraConditions = new List<string>();

        if (queryParams.Gender.HasValue)
        {
            extraConditions.Add("u.gender = @Gender");
        }

        if (queryParams.CourseId.HasValue)
        {
            extraConditions.Add("s.id IN (SELECT student_id FROM enrollments WHERE course_id = @CourseId)");
        }

        return ExecutePaginatedQueryAsync<StudentResponseDto>(
            queryParams,
            countSql: $"SELECT COUNT(1) {FromClause}",
            selectSql: $"SELECT {SelectColumns} {FromClause}",
            allowedSortColumns: AllowedSortColumns,
            defaultSortColumn: "s.created_at",
            searchCondition: "(u.first_name ILIKE @SearchTerm OR u.last_name ILIKE @SearchTerm OR u.email ILIKE @SearchTerm OR u.user_name ILIKE @SearchTerm)",
            extraConditions: extraConditions,
            configureParameters: parameters =>
            {
                if (queryParams.Gender.HasValue)
                {
                    parameters.Add("Gender", queryParams.Gender.Value);
                }
                if (queryParams.CourseId.HasValue)
                {
                    parameters.Add("CourseId", queryParams.CourseId.Value);
                }
            },
            ct);
    }
}
