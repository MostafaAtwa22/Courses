using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetByCourse
{
    public sealed class GetReviewsByCourseQueryHandler(IReviewRepository _repo)
        : IRequestHandler<GetReviewsByCourseQuery, PaginatedResult<ReviewResponseDto>>
    {
        public Task<PaginatedResult<ReviewResponseDto>> Handle(GetReviewsByCourseQuery request, CancellationToken ct)
        {
            return _repo.GetByCourseAsync(request.CourseId, request.QueryParams, ct);
        }
    }
}
