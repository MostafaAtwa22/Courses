using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetById
{
    public sealed record GetReviewByIdQuery(Guid Id) : IRequest<ReviewResponseDto?>;
}
