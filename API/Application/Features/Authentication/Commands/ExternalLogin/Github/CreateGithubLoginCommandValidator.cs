using FluentValidation;

namespace Application.Features.Authentication.Commands.ExternalLogin.Github;

public class CreateGithubLoginCommandValidator : AbstractValidator<CreateGithubLoginCommand>
{
    public CreateGithubLoginCommandValidator()
    {
        RuleFor(x => x.Dto.Code)
            .NotEmpty()
            .WithMessage("Github access code is required.");
        
        RuleFor(x => x.Dto.RedirectUri)
            .NotEmpty()
            .WithMessage("Github access Redirect URL is required.");
    }
}
