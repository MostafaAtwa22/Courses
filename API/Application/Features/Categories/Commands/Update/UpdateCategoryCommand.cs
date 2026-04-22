using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Commands.Update
{
    public sealed record UpdateCategoryCommand(Guid Id, CategoryUpdateDto Dto) : IRequest;
}