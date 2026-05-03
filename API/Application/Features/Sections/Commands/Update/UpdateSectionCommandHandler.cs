using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Features.Sections.Commands.Update
{
    public sealed class UpdateSectionCommandHandler(ISectionRepository _repo) : IRequestHandler<UpdateSectionCommand>
    {
        public async Task Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repo.GetEntityByIdAsync(request.Id, cancellationToken);
            
            if (section is null)
                throw new NotFoundException("Section", request.Id);

            request.Dto.UpdateEntity(section);
            await _repo.UpdateAsync(section, cancellationToken);
        }
    }
}
