using FluentValidation;

namespace Application.Features.Security.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Dto.Code)
                .NotEmpty().WithMessage("Verification code is required.")
                .Length(6).WithMessage("Verification code must be 6 digits.");
        }
    }
}
