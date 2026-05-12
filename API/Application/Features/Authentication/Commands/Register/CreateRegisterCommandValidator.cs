
namespace Application.Features.Authentication.Commands.Register
{
    public class CreateRegisterCommandValidator : AbstractValidator<CreateRegisterCommand>
    {
        public CreateRegisterCommandValidator()
        {
            RuleFor(v => v.Dto.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(v => v.Dto.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(v => v.Dto.UserName)
                .NotEmpty()
                .MaximumLength(50)
                .Matches(@"^[a-zA-Z0-9\-._@+]+$");

            RuleFor(v => v.Dto.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(v => v.Dto.Gender)
                .IsInEnum()
                .WithMessage("Invalid gender value. Allowed values are: Male, Female");

            RuleFor(v => v.Dto.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one digit")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(v => v.Dto.ConfirmPassword)
                .Equal(v => v.Dto.Password)
                .WithMessage("Confirm password must match the password")
                .NotEmpty();

            RuleFor(v => v.Dto.Role)
                .IsInEnum()
                .WithMessage("Invalid role value. Allowed values are: SuperAdmin, Admin, Instructor, Student");
        }
    }
}