using MediatR;

namespace Application.Features.Categories.Commands.Delete
{
    public sealed record DeleteCategoryCommand(Guid Id) : IRequest;
}
