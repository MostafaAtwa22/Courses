using Application.DTOs.Review;

namespace Application.Features.Reviews.Commands.Update
{
    public sealed record UpdateReviewCommand(Guid Id, ReviewUpdateDto Dto) : IRequest;
}
