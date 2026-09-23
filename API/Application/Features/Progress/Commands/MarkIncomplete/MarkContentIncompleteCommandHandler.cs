using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;

namespace Application.Features.Progress.Commands.MarkIncomplete
{
    public sealed class MarkContentIncompleteCommandHandler(
        IContentProgressRepository _progressRepo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<MarkContentIncompleteCommand>
    {
        public async Task Handle(MarkContentIncompleteCommand request, CancellationToken cancellationToken)
        {
            await _progressRepo.MarkIncompleteAsync(request.StudentId, request.Dto.ContentId, cancellationToken);

            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Progress(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProgressByUserCourse(userId, request.Dto.CourseId), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProgressByUser(userId), cancellationToken);
        }
    }
}
