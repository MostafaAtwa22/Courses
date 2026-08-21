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

            RuleFor(v => v.Dto.VideoFile)
                .NotNull();

            RuleFor(v => v.Dto.Attachments)
                .Must(attachments => attachments == null || attachments.Count <= 5)
                .WithMessage("Maximum 5 attachments allowed");

            RuleFor(v => v.Dto.SectionId)
                .NotEmpty();
        }
    }
}
