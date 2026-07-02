
namespace Application.Features.Authentication.Commands.ExternalLogin.Facebook
{
    public class CreateFacebookLoginCommandValidator : AbstractValidator<CreateFacebookLoginCommand>
    {
        public CreateFacebookLoginCommandValidator()
        {

            RuleFor(v => v.Dto.AccessToken)
                .NotEmpty();
        }
    }
}