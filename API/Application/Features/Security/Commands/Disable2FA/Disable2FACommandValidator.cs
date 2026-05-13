namespace Application.Features.Security.Commands.Disable2FA
{
    public class Disable2FACommandValidator : AbstractValidator<Disable2FACommand>
    {
        public Disable2FACommandValidator()
        {
            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password is required.");
            
            RuleFor(x => x.Dto.Code)
                .NotEmpty().WithMessage("Code is required.");
        }
    }
}