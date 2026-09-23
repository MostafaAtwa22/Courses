using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authorization.Queries.GetRoleByUserId;

public sealed record GetRoleByUserIdQueryHandler(
    IRoleRepository _roleRepo,
    UserManager<ApplicationUser> _userManager,
    IAppCache _cache) :
    IRequestHandler<GetRoleByUserIdQuery, UserRolesResponseDto>
{
    public async Task<UserRolesResponseDto> Handle(GetRoleByUserIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.RoleByUser(request.UserId);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(10),
            LocalCacheExpiration: TimeSpan.FromMinutes(2));

        var result = await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var user = await _userManager.FindByIdAsync(request.UserId)
                    ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(request.UserId));

                var roles = await _roleRepo.GetUserRolesAsync(request.UserId, ct);

                return user.ToUserRolesManageDto(roles);
            },
            cacheOptions,
            tags: new[] { CacheKeys.Roles() },
            cancellationToken);

        return result ?? new UserRolesResponseDto();
    }
}