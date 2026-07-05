using Application.DTOs.Course;

namespace Application.Features.Discount.Queries.GetDiscounts
{
    public sealed record GetCourseDiscountsQuery(Guid CourseId) : IRequest<IEnumerable<CourseDiscountDto>>;
}
