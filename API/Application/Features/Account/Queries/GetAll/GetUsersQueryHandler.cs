using Application.Common.Extensions;
using Application.Common.Mappings;
using Application.DTOs.Account;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries.GetAll
{
    public sealed class GetUsersQueryHandler(UserManager<ApplicationUser> _userManager)
        : IRequestHandler<GetUsersQuery, PaginatedResult<UserResponseDto>>
    {
        public async Task<PaginatedResult<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Params.PageNumber ?? 1;
            var pageSize = request.Params.PageSize ?? 10;

            var query = _userManager.Users.AsNoTracking()
                .Search(request.Params.SearchTerm)
                .FilterByGender(request.Params.Gender);

            query = await query.FilterByRoleAsync(_userManager, request.Params.Role);

            var (users, totalCount) = await query
                .OrderBy(u => u.Id)
                .PaginateAsync(pageNumber, pageSize, cancellationToken);

            var userDtos = new List<UserResponseDto>(users.Count);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(user.ToUserResponseDto(roles));
            }

            return new PaginatedResult<UserResponseDto>(userDtos, totalCount, pageNumber, pageSize);
        }
    }
}