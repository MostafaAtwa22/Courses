using MediatR;

namespace Application.Features.Discount.Commands.Delete
{
    public sealed record DeleteDiscountCommand(Guid Id) : IRequest;
}
