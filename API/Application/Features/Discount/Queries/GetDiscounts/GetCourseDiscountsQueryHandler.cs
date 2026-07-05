using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Discount.Queries.GetDiscounts
{
    public sealed class GetCourseDiscountsQueryHandler(
        ICourseDiscountRepository _discountRepository) : IRequestHandler<GetCourseDiscountsQuery, IEnumerable<CourseDiscountDto>>
    {
        public async Task<IEnumerable<CourseDiscountDto>> Handle(GetCourseDiscountsQuery request, CancellationToken cancellationToken)
        {
            return await _discountRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        }
    }
}
