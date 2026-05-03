using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Content;
using Dapper;
using Domain.Entities;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class ContentRepository(IDbConnectionFactory factory, IOptions<UrlsOptions> urlsOptions)
        : BaseRepository(factory), IContentRepository
    {
        private string SelectColumns =>
            $@"c.id, c.title, c.type, 
               CASE WHEN c.content_url IS NOT NULL 
                    THEN CONCAT('{urlsOptions.Value.API}/', c.content_url) 
                    ELSE NULL END AS content_url,
               c.order, c.is_preview, c.section_id, c.created_at, c.updated_at";

        private const string FromClause =
            @"FROM contents c
              JOIN sections s ON c.section_id = s.id";

        public Task<PaginatedResult<ContentResponseDto>> GetBySectionAsync(Guid sectionId, QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<ContentResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause} WHERE c.section_id = @SectionId",
                selectSql: $"SELECT {SelectColumns} {FromClause} WHERE c.section_id = @SectionId",
                allowedSortColumns: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "title", "c.title" },
                    { "order", "c.\"order\"" },
                    { "created_at", "c.created_at" }
                },
                defaultSortColumn: "c.\"order\"",
                searchCondition: "c.title ILIKE @SearchTerm",
                extraConditions: null,
                configureParameters: parameters =>
                {
                    parameters.Add("SectionId", sectionId);
                },
                ct);
        }

        public Task<PaginatedResult<ContentResponseDto>> GetByCourseAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default)
        {
            return ExecutePaginatedQueryAsync<ContentResponseDto>(
                queryParams,
                countSql: $"SELECT COUNT(1) {FromClause} WHERE s.course_id = @CourseId",
                selectSql: $"SELECT {SelectColumns} {FromClause} WHERE s.course_id = @CourseId",
                allowedSortColumns: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "title", "c.title" },
                    { "order", "c.\"order\"" },
                    { "created_at", "c.created_at" }
                },
                defaultSortColumn: "c.\"order\"",
                searchCondition: "c.title ILIKE @SearchTerm",
                extraConditions: null,
                configureParameters: parameters =>
                {
                    parameters.Add("CourseId", courseId);
                },
                ct);
        }

        public async Task<ContentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = $"SELECT {SelectColumns} {FromClause} WHERE c.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<ContentResponseDto>(sql, new { Id = id });
        }

        public async Task<Content?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            return await connection.QueryFirstOrDefaultAsync<Content>(
                "SELECT * FROM contents WHERE id = @Id", new { Id = id });
        }

        public async Task<Guid> CreateAsync(Content content, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"INSERT INTO contents (id, title, type, content_url, ""order"", is_preview, section_id, created_at, updated_at)
                        VALUES (@Id, @Title, @Type, @ContentUrl, @Order, @IsPreview, @SectionId, @CreatedAt, @UpdatedAt);";

            await connection.ExecuteAsync(sql, new
            {
                content.Id,
                content.Title,
                content.Type,
                content.ContentUrl,
                content.Order,
                content.IsPreview,
                content.SectionId,
                content.CreatedAt,
                content.UpdatedAt
            });
            return content.Id;
        }

        public async Task UpdateAsync(Content content, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"UPDATE contents
                        SET title       = @Title,
                            type        = @Type,
                            content_url = @ContentUrl,
                            ""order""     = @Order,
                            is_preview  = @IsPreview,
                            section_id  = @SectionId,
                            updated_at  = @UpdatedAt
                        WHERE id = @Id;";

            await connection.ExecuteAsync(sql, new
            {
                content.Id,
                content.Title,
                content.Type,
                content.ContentUrl,
                content.Order,
                content.IsPreview,
                content.SectionId,
                content.UpdatedAt
            });
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            await connection.ExecuteAsync("DELETE FROM contents WHERE id = @Id;", new { Id = id });
        }
    }
}
