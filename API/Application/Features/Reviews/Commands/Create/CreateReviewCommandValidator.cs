using FluentValidation;

namespace Application.Features.Reviews.Commands.Create
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(v => v.Dto.Headline)
                .NotEmpty().WithMessage("Headline is required.")
                .MaximumLength(200).WithMessage("Headline must not exceed 200 characters.");

            RuleFor(v => v.Dto.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");

            RuleFor(v => v.Dto.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(v => v.Dto.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
        }
    }
}
