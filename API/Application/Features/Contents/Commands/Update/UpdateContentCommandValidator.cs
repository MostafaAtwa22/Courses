using FluentValidation;

namespace Application.Features.Contents.Commands.Update
{
    public class UpdateContentCommandValidator : AbstractValidator<UpdateContentCommand>
    {
        public UpdateContentCommandValidator()
        {
            RuleFor(v => v.Dto.Title)
                .MaximumLength(200)
                .NotEmpty();

            RuleFor(v => v.Dto.Type)
                .IsInEnum();

            RuleFor(v => v.Dto.SectionId)
                .NotEmpty();
        }
    }
}
