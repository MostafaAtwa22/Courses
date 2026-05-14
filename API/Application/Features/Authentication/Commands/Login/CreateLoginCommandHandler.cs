using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Constant = Domain.Constants.IdentityConstants;

namespace Application.Features.Authentication.Commands.Login
{
    public sealed class CreateLoginCommandHandler(
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IAuthService _authService,
            ITwoFactorService _twoFactorService) :
        IRequestHandler<CreateLoginCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(CreateLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("Invalid email or password.");
            
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new EmailNotConfirmedException("Email confirmation is required.");
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, true);
            
            if (result.IsLockedOut)
                throw new AccountLockedException("Account is locked due to multiple failed login attempts. Please try again later.");

            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid email or password.");

            if (result.RequiresTwoFactor)
            {
                await _twoFactorService.SendOtpAsync(user);
                return new AuthResponseDto
                {
                    Email = user.Email!,
                    RequiresTwoFactor = true,
                    Provider = Constant.EmailOtpProvider
                };
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await _authService.CreateTokenAsync(user);

            var returnUser = user.ToAuthResponseDto(await _userManager.GetRolesAsync(user));
            returnUser.Token = token;
            return returnUser;
        }
    }
}