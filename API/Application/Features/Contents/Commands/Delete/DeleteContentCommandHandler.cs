using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Contents.Commands.Delete
{
    public sealed class DeleteContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService)
        : IRequestHandler<DeleteContentCommand>
    {
        public async Task Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            await _fileService.DeleteAsync(content.ContentUrl);
            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
