using Application.Common.Interfaces.Cache;

namespace Application.Features.Student.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(
    IStudentRepository _studentRepository,
    IAppCache _cache)
    : IRequestHandler<DeleteStudentCommand>
{
    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        await _studentRepository.DeleteAsync(request.Id, cancellationToken);

        // Invalidate related caches
        await _cache.RemoveByTagAsync(CacheKeys.Students(), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Student(request.Id), cancellationToken);
        await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        await _cache.RemoveByTagAsync(CacheKeys.Progress(), cancellationToken);
    }
}
