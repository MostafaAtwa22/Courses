using Application.Common.Interfaces;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Features.Sections.Commands.Delete
{
    public sealed class DeleteSectionCommandHandler(ISectionRepository _repo) : IRequestHandler<DeleteSectionCommand>
    {
        public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repo.GetEntityByIdAsync(request.Id, cancellationToken);
            
            if (section is null)
                throw new NotFoundException("Section", request.Id);

            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
