using Application.Common.Models;
using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetByCourse
{
    public sealed record GetReviewsByCourseQuery(Guid CourseId, QueryParams QueryParams)
        : IRequest<PaginatedResult<ReviewResponseDto>>;
}
