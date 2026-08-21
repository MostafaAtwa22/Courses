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

            RuleFor(v => v.Dto.AttachmentsToAdd)
                .Must(attachments => attachments == null || attachments.Count <= 5)
                .WithMessage("Maximum 5 attachments allowed");

            RuleFor(v => v.Dto.SectionId)
                .NotEmpty();
        }
    }
}
