using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories.Commands.Update
{
    public sealed class UpdateCategoryCommandHandler(ICategoryRepository _repo) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repo.GetByIdAsync(request.Id, cancellationToken) 
                ?? throw new NotFoundException(nameof(Category), request.Id);

            await _repo.UpdateAsync(request.Id, request.Dto, cancellationToken);
        }
    }
}