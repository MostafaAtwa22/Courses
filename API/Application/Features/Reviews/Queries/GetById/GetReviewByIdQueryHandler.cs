using Application.Common.Interfaces;
using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetById
{
    public sealed class GetReviewByIdQueryHandler(IReviewRepository _repo)
        : IRequestHandler<GetReviewByIdQuery, ReviewResponseDto?>
    {
        public Task<ReviewResponseDto?> Handle(GetReviewByIdQuery request, CancellationToken ct)
        {
            return _repo.GetByIdAsync(request.Id, ct);
        }
    }
}
