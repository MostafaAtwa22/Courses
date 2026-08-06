using FluentValidation;

namespace Application.Features.Progress.Commands.MarkIncomplete
{
    public class MarkContentIncompleteCommandValidator : AbstractValidator<MarkContentIncompleteCommand>
    {
        public MarkContentIncompleteCommandValidator()
        {
            RuleFor(x => x.Dto.ContentId)
                .NotEmpty().WithMessage("ContentId is required.");

            RuleFor(x => x.Dto.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
        }
    }
}
