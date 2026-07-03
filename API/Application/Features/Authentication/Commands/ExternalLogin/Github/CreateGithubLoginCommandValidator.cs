using FluentValidation;

namespace Application.Features.Authentication.Commands.ExternalLogin.Github;

public class CreateGithubLoginCommandValidator : AbstractValidator<CreateGithubLoginCommand>
{
    public CreateGithubLoginCommandValidator()
    {
        RuleFor(x => x.Dto.AccessToken)
            .NotEmpty()
            .WithMessage("Github access token is required.");
    }
}
