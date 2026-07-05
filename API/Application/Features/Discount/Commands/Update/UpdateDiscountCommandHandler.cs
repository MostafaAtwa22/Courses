using Application.Common.Interfaces;
using Application.Common.Exceptions;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Discount.Commands.Update
{
    public sealed class UpdateDiscountCommandHandler(
        ICourseDiscountRepository _discountRepository) : IRequestHandler<UpdateDiscountCommand>
    {
        public async Task Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _discountRepository.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("CourseDiscount", request.Id);

            request.Dto.UpdateEntity(discount);

            await _discountRepository.UpdateAsync(discount, cancellationToken);
        }
    }
}
