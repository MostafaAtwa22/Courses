using Application.Common.Interfaces;
using MediatR;
using Application.Common.Mappings;

namespace Application.Features.Categories.Commands.Create
{
    public sealed class CreateCategoryCommandHandler(ICategoryRepository _repo) : IRequestHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = request.Dto.ToEntity();
            return await _repo.CreateAsync(category, cancellationToken);
        }
    }
}