using Application.Common.Mappings;
using Application.DTOs.Account;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler(UserManager<ApplicationUser> _userManager)
        : IRequestHandler<GetUserByIdQuery, UserResponseDto>
    {
        public async Task<UserResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.Id.ToString(), cancellationToken)
                ?? throw new NotFoundException(nameof(ApplicationUser), request.Id);

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToUserResponseDto(roles);
        }
    }
}