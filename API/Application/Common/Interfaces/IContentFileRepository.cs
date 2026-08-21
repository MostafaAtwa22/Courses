using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IContentFileRepository
    {
        Task<Guid> CreateAsync(ContentFile contentFile, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<ContentFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ContentFile>> GetByContentIdAsync(Guid contentId, CancellationToken ct = default);
    }
}
