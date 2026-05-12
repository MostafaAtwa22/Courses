using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.Update;
using Application.Features.Profiles.Commands.Delete;
using FluentValidation.TestHelper;

namespace Application.Tests.Profiles.Validators;

public class ProfileValidatorTests
{
    private readonly UpdateProfileCommandValidator _updateValidator;
    private readonly DeleteProfileCommandValidator _deleteValidator;

    public ProfileValidatorTests()
    {
        _updateValidator = new UpdateProfileCommandValidator();
        _deleteValidator = new DeleteProfileCommandValidator();
    }

    [Fact]
    public void UpdateProfileValidator_ShouldHaveError_WhenRequiredFieldsAreEmpty()
    {
        var dto = new UpdateProfileDto();
        var command = new UpdateProfileCommand(dto);
        var result = _updateValidator.TestValidate(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Dto.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.Dto.LastName);
        result.ShouldHaveValidationErrorFor(x => x.Dto.UserName);
    }

    [Fact]
    public void UpdateProfileValidator_ShouldHaveError_WhenPhoneNumberIsInvalid()
    {
        var dto = new UpdateProfileDto { PhoneNumber = "invalid" };
        var command = new UpdateProfileCommand(dto);
        var result = _updateValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.PhoneNumber);
    }

    [Fact]
    public void DeleteProfileValidator_ShouldHaveError_WhenPasswordIsEmpty()
    {
        var dto = new DeleteProfileDto { Password = "" };
        var command = new DeleteProfileCommand(dto);
        var result = _deleteValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Dto.Password);
    }
}
