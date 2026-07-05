namespace Application.Features.Discount.Commands.Create
{
    public sealed record CreateDiscountCommand(Guid CourseId, CreateCourseDiscountDto Dto) : IRequest<Guid>;
}
