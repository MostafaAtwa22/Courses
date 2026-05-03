using Application.Common.Interfaces;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Sections.Commands.Create
{
    public sealed class CreateSectionCommandHandler(ISectionRepository _repo) : IRequestHandler<CreateSectionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = request.Dto.ToEntity();
            return await _repo.CreateAsync(section, cancellationToken);
        }
    }
}
