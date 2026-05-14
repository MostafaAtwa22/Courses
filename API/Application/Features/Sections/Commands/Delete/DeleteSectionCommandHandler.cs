namespace Application.Features.Sections.Commands.Delete
{
    public sealed class DeleteSectionCommandHandler(ISectionRepository _repo) : IRequestHandler<DeleteSectionCommand>
    {
        public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            if (await _repo.GetEntityByIdAsync(request.Id, cancellationToken) is null)
                throw new NotFoundException("Section", request.Id);

            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
