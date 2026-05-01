using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;

namespace Application.Features.Reviews.Commands.Update
{
    public sealed class UpdateReviewCommandHandler(
        IReviewRepository _repo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<UpdateReviewCommand>
    {
        public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in to update a review.");

            var studentId = await _repo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can update reviews.");

            var review = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Review), request.Id);

            if (review.StudentId != studentId)
                throw new ForbiddenException("You can only update your own reviews.");

            request.Dto.UpdateEntity(review);

            await _repo.UpdateAsync(review, cancellationToken);
        }
    }
}
