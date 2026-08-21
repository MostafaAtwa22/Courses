using Application.DTOs.Review;
using Domain.Entities.Identity;

namespace Application.Features.Reviews.Queries.GetUserReview
{
    public sealed class GetUserReviewQueryHandler(
        IReviewRepository _repo)
        : IRequestHandler<GetUserReviewQuery, ReviewResponseDto?>
    {
        public async Task<ReviewResponseDto?> Handle(GetUserReviewQuery request, CancellationToken ct)
        {
            var user = request.User;
            var studentId = await _repo.GetStudentIdByUserIdAsync(user!.Id.ToString(), ct)
                            ?? throw new NotFoundException($"Student", Guid.Parse(user.Id)); 

            var review = await _repo.GetByUserAndCourseAsync(studentId, request.CourseId, ct);
            return review;
        }
    }
}
