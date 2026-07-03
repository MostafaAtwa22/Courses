using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Commands.Login;

public sealed class CreateLoginCommandHandler(
    UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager,
    ILoginPipeline _loginPipeline) :
    IRequestHandler<CreateLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(CreateLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Dto.Email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new EmailNotConfirmedException("Email confirmation is required.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid email or password.");

        return await _loginPipeline.ExecuteAsync(user);
    }
}
