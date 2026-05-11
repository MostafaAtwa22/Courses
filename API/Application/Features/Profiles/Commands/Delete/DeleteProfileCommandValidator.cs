namespace Application.Features.Profiles.Commands.Delete
{
    public class DeleteProfileCommandValidator : AbstractValidator<DeleteProfileCommand>
    {
        public DeleteProfileCommandValidator()
        {
            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}