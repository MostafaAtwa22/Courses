namespace Application.Features.Account.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Dto.Token)
                .NotEmpty().WithMessage("Password reset token is required.");

            RuleFor(v => v.Dto.NewPassword)
                .NotEmpty()
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("New password must contain at least one digit")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");


            RuleFor(x => x.Dto.ConfirmNewPassword)
                .Equal(x => x.Dto.NewPassword)
                .WithMessage("Passwords do not match");
        }
    }
}
