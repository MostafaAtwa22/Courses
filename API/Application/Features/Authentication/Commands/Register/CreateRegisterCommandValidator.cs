using Application.Common.Validation;
using Domain.Enums.Identity;

namespace Application.Features.Authentication.Commands.Register
{
    public class CreateRegisterCommandValidator : AbstractValidator<CreateRegisterCommand>
    {
        public CreateRegisterCommandValidator()
        {
            RuleFor(v => v.Dto.FirstName)
                .ApplyFirstNameValidation();

            RuleFor(v => v.Dto.LastName)
                .ApplyLastNameValidation();

            RuleFor(v => v.Dto.UserName)
                .ApplyUserNameValidation();

            RuleFor(v => v.Dto.Email)
                .ApplyEmailValidation();

            RuleFor(v => v.Dto.Gender)
                .ApplyGenderValidation();

            RuleFor(v => v.Dto.Password)
                .ApplyPasswordValidation();

            RuleFor(v => v.Dto.ConfirmPassword)
                .Equal(v => v.Dto.Password)
                .WithMessage("Confirm password must match the password")
                .NotEmpty();

            RuleFor(v => v.Dto.Role)
                .ApplyRoleValidation(
                    new[] { Role.Student, Role.Instructor },
                    "Registration is only allowed for Student or Instructor roles.");
        }
    }
}