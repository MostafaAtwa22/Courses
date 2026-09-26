using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;

namespace Application.Features.Admin.Commands.Delete;

public sealed class DeleteAdminCommandHandler(
    IAdminRepository _adminRepository,
    IAppCache _cache)
    : IRequestHandler<DeleteAdminCommand>
{
    public async Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
    {
        if (await _adminRepository.IsLastSuperAdminAsync(cancellationToken))
            throw new BadRequestException("Cannot delete the last SuperAdmin user.");

        await _adminRepository.DeleteAsync(request.Id, cancellationToken);

        await _cache.RemoveByTagAsync(CacheKeys.Admins(), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Admin(request.Id), cancellationToken);
    }
}