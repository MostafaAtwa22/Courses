using FluentValidation;

namespace Application.Features.Security.Commands.VerifyTwoFactor
{
    public class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
    {
        public VerifyTwoFactorCommandValidator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Dto.Code)
                .NotEmpty().WithMessage("Verification code is required.")
                .Length(6).WithMessage("Verification code must be 6 digits.");
        }
    }
}
