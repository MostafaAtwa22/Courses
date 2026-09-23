using Application.Common.Extensions;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Account;

namespace Application.Features.Account.Queries.GetAll
{
    public sealed class GetUsersQueryHandler(
        UserManager<ApplicationUser> _userManager, 
        IUrlProvider _urlProvider,
        IAppCache _cache)
        : IRequestHandler<GetUsersQuery, PaginatedResult<UserResponseDto>>
    {
        public async Task<PaginatedResult<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Params.PageNumber ?? 1;
            var pageSize = request.Params.PageSize ?? 10;

            var cacheKey = CacheKeys.Users(
                request.Params.SearchTerm ?? string.Empty,
                request.Params.Gender?.ToString() ?? string.Empty,
                request.Params.Role ?? string.Empty,
                pageNumber,
                pageSize);

            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var query = _userManager.Users.AsNoTracking()
                        .Search(request.Params.SearchTerm)
                        .FilterByGender(request.Params.Gender);

                    query = await query.FilterByRoleAsync(_userManager, request.Params.Role);

                    var (users, totalCount) = await query
                        .OrderBy(u => u.Id)
                        .PaginateAsync(pageNumber, pageSize, ct);

                    var userDtos = new List<UserResponseDto>(users.Count);

                    foreach (var user in users)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        userDtos.Add(user.ToUserResponseDto(roles, _urlProvider));
                    }

                    return new PaginatedResult<UserResponseDto>(userDtos, totalCount, pageNumber, pageSize);
                },
                cacheOptions,
                tags: new[] { CacheKeys.Users() },
                cancellationToken);

            return result ?? new PaginatedResult<UserResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}