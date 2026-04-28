using Application.Common.Exceptions;
using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Features.Categories.Commands.Update
{
    public sealed class UpdateCategoryCommandHandler(ICategoryRepository _repo) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repo.GetEntityByIdAsync(request.Id, cancellationToken) 
                ?? throw new NotFoundException(nameof(Category), request.Id);

            request.Dto.UpdateEntity(category);

            await _repo.UpdateAsync(category, cancellationToken);
        }
    }
}