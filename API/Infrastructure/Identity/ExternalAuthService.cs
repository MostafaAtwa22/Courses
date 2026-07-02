using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Identity;
using Application.DTOs.Authentication;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;
using IdentityConstants = Domain.Constants.IdentityConstants;

namespace Infrastructure.Identity;

public class ExternalAuthService : IExternalAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEnumerable<IExternalLoginValidator> _validators;

    public ExternalAuthService(
        UserManager<ApplicationUser> userManager,
        IEnumerable<IExternalLoginValidator> validators)
    {
        _userManager = userManager;
        _validators = validators;
    }

    public async Task<ApplicationUser> GoogleLoginAsync(GoogleLoginDto googleLoginDto)
    {
        var validator = _validators.FirstOrDefault(v => v.Provider == ExternalLoginProvider.Google)
            ?? throw new InvalidOperationException("Google login validator not found.");

        var externalUser = await validator.ValidateAsync(googleLoginDto.IdToken);

        return await ProcessExternalUserAsync(externalUser, IdentityConstants.Google);
    }

    public async Task<ApplicationUser> FacebookLoginAsync(FacebookLoginDto facebookLoginDto)
    {
        var validator = _validators.FirstOrDefault(v => v.Provider == ExternalLoginProvider.Facebook)
            ?? throw new InvalidOperationException("Facebook login validator not found.");

        var externalUser = await validator.ValidateAsync(facebookLoginDto.AccessToken);

        return await ProcessExternalUserAsync(externalUser, "Facebook");
    }

    private async Task<ApplicationUser> ProcessExternalUserAsync(ExternalUser externalUser, string providerStr)
    {
        var user = await _userManager.FindByLoginAsync(providerStr, externalUser.Id);

        if (user is not null)
            return user;

        user = externalUser.MapToApplicationUser();

        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
            throw new Exception($"Failed to create user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, Role.Student.ToString());

        var loginInfo = new UserLoginInfo(providerStr, externalUser.Id, providerStr);
        
        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!addLoginResult.Succeeded)
            throw new Exception($"Failed to add external login: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");

        return user;
    }
}