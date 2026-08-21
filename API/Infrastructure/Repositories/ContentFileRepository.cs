using Application.Common.Interfaces;
using Application.Common.Options;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class ContentFileRepository(IDbConnectionFactory factory)
        : BaseRepository(factory), IContentFileRepository
    {
        public async Task<Guid> CreateAsync(ContentFile contentFile, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = @"INSERT INTO content_files (id, file_name, file_url, content_id, created_at, updated_at)
                        VALUES (@Id, @FileName, @FileUrl, @ContentId, @CreatedAt, @UpdatedAt);";

            await connection.ExecuteAsync(sql, new
            {
                contentFile.Id,
                contentFile.FileName,
                contentFile.FileUrl,
                contentFile.ContentId,
                contentFile.CreatedAt,
                contentFile.UpdatedAt
            });
            return contentFile.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            await connection.ExecuteAsync("DELETE FROM content_files WHERE id = @Id;", new { Id = id });
        }

        public async Task<ContentFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            return await connection.QueryFirstOrDefaultAsync<ContentFile>(
                "SELECT * FROM content_files WHERE id = @Id", new { Id = id });
        }

        public async Task<IReadOnlyList<ContentFile>> GetByContentIdAsync(Guid contentId, CancellationToken ct = default)
        {
            using var connection = await CreateConnectionAsync(ct);
            var sql = "SELECT * FROM content_files WHERE content_id = @ContentId ORDER BY created_at ASC";
            return (await connection.QueryAsync<ContentFile>(sql, new { ContentId = contentId })).AsList();
        }
    }
}
