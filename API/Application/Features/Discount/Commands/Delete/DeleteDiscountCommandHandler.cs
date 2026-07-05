using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Discount.Commands.Delete
{
    public sealed class DeleteDiscountCommandHandler(
        ICourseDiscountRepository _discountRepository) : IRequestHandler<DeleteDiscountCommand>
    {
        public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _discountRepository.GetEntityByIdAsync(request.Id, cancellationToken);
            if (discount is null)
                throw new NotFoundException("CourseDiscount", request.Id);

            await _discountRepository.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
