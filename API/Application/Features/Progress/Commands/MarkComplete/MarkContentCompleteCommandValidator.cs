using FluentValidation;

namespace Application.Features.Progress.Commands.MarkComplete
{
    public class MarkContentCompleteCommandValidator : AbstractValidator<MarkContentCompleteCommand>
    {
        public MarkContentCompleteCommandValidator()
        {
            RuleFor(x => x.Dto.ContentId)
                .NotEmpty().WithMessage("ContentId is required.");

            RuleFor(x => x.Dto.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
        }
    }
}
