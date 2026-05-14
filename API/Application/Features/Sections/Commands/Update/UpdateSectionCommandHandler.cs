namespace Application.Features.Sections.Commands.Update
{
    public sealed class UpdateSectionCommandHandler(ISectionRepository _repo) : IRequestHandler<UpdateSectionCommand>
    {
        public async Task Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Section", request.Id);

            request.Dto.UpdateEntity(section);
            await _repo.UpdateAsync(section, cancellationToken);
        }
    }
}
