using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Mappings;
using Application.DTOs.Account;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(
        UserManager<ApplicationUser> _userManager,
        IUrlProvider _urlProvider,
        IAppCache _cache)
        : IRequestHandler<GetUserByIdQuery, UserResponseDto>
    {
        public async Task<UserResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.User(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(1),
                LocalCacheExpiration: TimeSpan.FromSeconds(30));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var user = await _userManager.Users.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == request.Id.ToString(), ct)
                        ?? throw new NotFoundException(nameof(ApplicationUser), request.Id);

                    var roles = await _userManager.GetRolesAsync(user);
                    return user.ToUserResponseDto(roles, _urlProvider);
                },
                cacheOptions,
                tags: new[] { CacheKeys.Users() },
                cancellationToken);
            
            return result!;
        }
    }
}