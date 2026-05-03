using FluentValidation;

namespace Application.Features.Contents.Commands.Create
{
    public class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
    {
        public CreateContentCommandValidator()
        {
            RuleFor(v => v.Dto.Title)
                .MaximumLength(200)
                .NotEmpty();

            RuleFor(v => v.Dto.Type)
                .IsInEnum();

            RuleFor(v => v.Dto.SectionId)
                .NotEmpty();
                
            RuleFor(v => v.Dto.File)
                .NotNull();
        }
    }
}
