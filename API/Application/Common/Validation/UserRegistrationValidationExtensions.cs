using Domain.Enums.Identity;

namespace Application.Common.Validation;

public static class UserRegistrationValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyFirstNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(50);
    }

    public static IRuleBuilderOptions<T, string> ApplyLastNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(50);
    }

    public static IRuleBuilderOptions<T, string> ApplyUserNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9\-._@+]+$");
    }

    public static IRuleBuilderOptions<T, string> ApplyEmailValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress().WithMessage("A valid email address is required.")
            .NotEmpty();
    }

    public static IRuleBuilderOptions<T, Gender> ApplyGenderValidation<T>(this IRuleBuilder<T, Gender> ruleBuilder)
    {
        return ruleBuilder
            .IsInEnum()
            .WithMessage("Invalid gender value. Allowed values are: Male, Female");
    }

    public static IRuleBuilderOptions<T, string> ApplyPasswordValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }

    public static IRuleBuilderOptions<T, Role> ApplyRoleValidation<T>(this IRuleBuilder<T, Role> ruleBuilder, Role[] allowedRoles, string errorMessage)
    {
        return ruleBuilder
            .Must(r => allowedRoles.Contains(r))
            .WithMessage(errorMessage);
    }
}