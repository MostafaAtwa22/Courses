using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Categories.Commands.Create
{
    public sealed class CreateCategoryCommandHandler(ICategoryRepository _repo) : IRequestHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _repo.CreateAsync(request.Dto, cancellationToken);
        }
    }
}