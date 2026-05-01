using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Features.Reviews.Commands.Delete
{
    public sealed class DeleteReviewCommandHandler(
        IReviewRepository _repo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<DeleteReviewCommand>
    {
        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in to delete a review.");

            var studentId = await _repo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can delete reviews.");

            var review = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Review), request.Id);

            if (review.StudentId != studentId)
                throw new ForbiddenException("You can only delete your own reviews.");

            await _repo.DeleteAsync(review.Id, cancellationToken);
        }
    }
}
