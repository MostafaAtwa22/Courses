using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;

namespace Application.Features.Progress.Commands.MarkComplete
{
    public sealed class MarkContentCompleteCommandHandler(
        IContentProgressRepository _progressRepo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<MarkContentCompleteCommand>
    {
        public async Task Handle(MarkContentCompleteCommand request, CancellationToken cancellationToken)
        {
            await _progressRepo.MarkCompleteAsync(request.StudentId, request.Dto.ContentId, request.Dto.CourseId, cancellationToken);

            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Progress(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProgressByUserCourse(userId, request.Dto.CourseId), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProgressByUser(userId), cancellationToken);
        }
    }
}
