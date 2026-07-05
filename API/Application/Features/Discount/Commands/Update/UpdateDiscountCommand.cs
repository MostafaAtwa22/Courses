using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Discount.Commands.Update
{
    public sealed record UpdateDiscountCommand(Guid Id, UpdateCourseDiscountDto Dto) : IRequest;
}
