using Application.DTOs.Review;

namespace Application.Features.Reviews.Commands.Create
{
    public sealed record CreateReviewCommand(ReviewCreateDto Dto) : IRequest<Guid>;
}
