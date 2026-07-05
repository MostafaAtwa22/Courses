using Application.Common.Interfaces;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Discount.Commands.Create
{
    public sealed class CreateDiscountCommandHandler(
        ICourseRepository _courseRepository,
        ICourseDiscountRepository _discountRepository) : IRequestHandler<CreateDiscountCommand, Guid>
    {
        public async Task<Guid> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetEntityByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course", request.CourseId);

            var discount = request.Dto.ToEntity(request.CourseId);

            return await _discountRepository.AddAsync(discount, cancellationToken);
        }
    }
}
