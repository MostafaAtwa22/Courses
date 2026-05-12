using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.ChangePassword;
using Application.Features.Profiles.Commands.SetPassword;
using FluentValidation.TestHelper;

namespace Application.Tests.Profiles.Validators;

public class PasswordValidatorTests
{
    private readonly ChangePasswordCommandValidator _changeValidator;
    private readonly SetPasswordCommandValidator _setValidator;

    public PasswordValidatorTests()
    {
        _changeValidator = new ChangePasswordCommandValidator();
        _setValidator = new SetPasswordCommandValidator();
    }

    [Fact]
    public void ChangePasswordValidator_ShouldHaveError_WhenOldPasswordIsEmpty()
    {
        var dto = new ChangePasswordDto { OldPassword = "" };
        var command = new ChangePasswordCommand(dto);
        var result = _changeValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.OldPassword);
    }

    [Theory]
    [InlineData("short")] // too short
    [InlineData("nouppercase1!")] // no uppercase
    [InlineData("NOLOWERCASE1!")] // no lowercase
    [InlineData("NoDigit!")] // no digit
    [InlineData("NoSpecial1")] // no special character
    public void ChangePasswordValidator_ShouldHaveError_WhenNewPasswordIsInvalid(string password)
    {
        var dto = new ChangePasswordDto 
        { 
            OldPassword = "ValidOld1!",
            NewPassword = password,
            ConfirmNewPassword = password
        };
        var command = new ChangePasswordCommand(dto);
        var result = _changeValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.NewPassword);
    }

    [Fact]
    public void ChangePasswordValidator_ShouldHaveError_WhenPasswordsDoNotMatch()
    {
        var dto = new ChangePasswordDto 
        { 
            OldPassword = "ValidOld1!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "Different123!"
        };
        var command = new ChangePasswordCommand(dto);
        var result = _changeValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.ConfirmNewPassword);
    }

    [Fact]
    public void SetPasswordValidator_ShouldHaveError_WhenNewPasswordIsInvalid()
    {
        var dto = new SetPasswordDto { NewPassword = "short" };
        var command = new SetPasswordCommand(dto);
        var result = _setValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.NewPassword);
    }

    [Fact]
    public void SetPasswordValidator_ShouldNotHaveError_WhenDataIsValid()
    {
        var dto = new SetPasswordDto 
        { 
            NewPassword = "ValidPassword123!",
            ConfirmNewPassword = "ValidPassword123!"
        };
        var command = new SetPasswordCommand(dto);
        var result = _setValidator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
