using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Commands.Login
{
    public sealed class CreateLoginCommandHandler(
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IAuthService _authService) :
        IRequestHandler<CreateLoginCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(CreateLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");
            
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new UnauthorizedAccessException("Email confirmation is required.");
            
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
                throw new UnauthorizedAccessException("Account is locked. Please try again later.");
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, true);
            
            if (result.IsLockedOut)
                throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts. Please try again later.");

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");
            
            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await _authService.CreateTokenAsync(user);

            var returnUser = user.ToAuthResponseDto(await _userManager.GetRolesAsync(user));
            returnUser.Token = token;
            return returnUser;
        }
    }
}