using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;
using IdentityConstants = Domain.Constants.IdentityConstants;


namespace Infrastructure.Identity;

public class ExternalAuthService(
    UserManager<ApplicationUser> _userManager,
    IExternalLogin _externalLogin) : IExternalAuthService
{
    public async Task<ApplicationUser> GoogleLoginAsync(GoogleLoginDto googleLoginDto)
    {
        var googleUser = await _externalLogin.ValidateGoogleAsync(googleLoginDto.IdToken);

        if (googleUser is null)
            throw new UnauthorizedException("Invalid Google token.");

        var user = await _userManager.FindByLoginAsync(IdentityConstants.Google, 
            googleUser.Subject);

        if (user is not null)
            return user;

        user = googleUser.MapFromGoogle();

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
            throw new Exception($"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, Role.Student.ToString());

        var loginInfo = new UserLoginInfo(
            IdentityConstants.Google,
            googleUser.Subject,
            IdentityConstants.Google);
        
        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!addLoginResult.Succeeded)
            throw new Exception($"Failed to add Google login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");

        return user;
    }
}