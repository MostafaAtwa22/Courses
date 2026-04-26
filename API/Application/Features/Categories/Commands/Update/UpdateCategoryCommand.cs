using Application.DTOs.Category;

namespace Application.Features.Categories.Commands.Update
{
    public sealed record UpdateCategoryCommand(Guid Id, CategoryUpdateDto Dto) : IRequest;
}