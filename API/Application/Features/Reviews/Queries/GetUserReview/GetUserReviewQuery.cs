using Application.DTOs.Review;
using Domain.Entities.Identity;

namespace Application.Features.Reviews.Queries.GetUserReview
{
    public sealed record GetUserReviewQuery(Guid CourseId) : IRequest<ReviewResponseDto?>, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }
}
