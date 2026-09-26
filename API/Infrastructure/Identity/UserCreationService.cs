using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Mappings;
using Application.DTOs.Authentication;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class UserCreationService(
    UserManager<ApplicationUser> _userManager,
    IUserIdentityService _userIdentityService,
    IPasswordService _passwordService,
    IIdentityEmailService _identityEmailService,
    IStudentProfileService _studentProfileService) : IUserCreationService
{
    public async Task<ApplicationUser> CreateUserAsync(
        RegisterDto registerDto,
        UserCreationOptions options,
        CancellationToken cancellationToken = default)
    {
        if (await _userIdentityService.IsEmailExistsAsync(registerDto.Email))
            throw new BadRequestException("Email already exists.");

        if (await _userIdentityService.IsUserNameExistsAsync(registerDto.UserName))
            throw new BadRequestException("Username already exists.");

        var user = registerDto.ToApplicationUser();

        if (options.AutoConfirmEmail)
            user.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());

        if (options.CreateStudentProfile && registerDto.Role == Role.Student)
            await _studentProfileService.EnsureStudentProfileAsync(user.Id, cancellationToken);

        if (options.ConfirmEmail && !options.AutoConfirmEmail)
        {
            var token = await _passwordService.GenerateEmailConfirmationTokenAsync(user);
            await _identityEmailService.SendEmailConfirmationEmailAsync(user, token);
        }

        return user;
    }
}