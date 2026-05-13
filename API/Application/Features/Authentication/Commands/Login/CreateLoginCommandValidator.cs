
namespace Application.Features.Authentication.Commands.Login
{
    public class CreateLoginCommandValidator : AbstractValidator<CreateLoginCommand>
    {
        public CreateLoginCommandValidator()
        {

            RuleFor(v => v.Dto.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(v => v.Dto.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}