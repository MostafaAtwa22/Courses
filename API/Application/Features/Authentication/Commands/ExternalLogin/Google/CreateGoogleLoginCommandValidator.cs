
namespace Application.Features.Authentication.Commands.ExternalLogin.Google
{
    public class CreateGoogleLoginCommandValidator : AbstractValidator<CreateGoogleLoginCommand>
    {
        public CreateGoogleLoginCommandValidator()
        {

            RuleFor(v => v.Dto.IdToken)
                .NotEmpty();
        }
    }
}