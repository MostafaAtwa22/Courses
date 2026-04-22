using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Commands.Create
{
    public sealed record CreateCategoryCommand(CategoryCreateDto Dto) : IRequest<Guid>;
}