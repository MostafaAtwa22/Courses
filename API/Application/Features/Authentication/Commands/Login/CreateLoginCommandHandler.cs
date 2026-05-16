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
                throw new AccountLockedException("Account is locked. Please try again later.");

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

            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid email or password.");

            return await _authService.GetAuthResponseAsync(user);
        }
    }
}