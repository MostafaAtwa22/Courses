using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces
{
    public interface IContentAttachmentService
    {
        Task<List<ContentFile>> UploadAttachmentsAsync(Guid contentId, List<IFormFile> attachments, CancellationToken cancellationToken);
        Task DeleteAttachmentsAsync(List<Guid> attachmentIds, CancellationToken cancellationToken);
        Task DeleteAllAttachmentsAsync(Guid contentId, CancellationToken cancellationToken);
        Task ValidateAttachmentCountAsync(Guid contentId, int newAttachmentsCount, int attachmentsToRemoveCount, CancellationToken cancellationToken);
    }
}
