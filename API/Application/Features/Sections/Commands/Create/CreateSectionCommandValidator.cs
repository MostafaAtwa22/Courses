using FluentValidation;

namespace Application.Features.Sections.Commands.Create
{
    public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
    {
        public CreateSectionCommandValidator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters.")
                .MinimumLength(3)
                .WithMessage("Title must be at least 3 characters long.");

            RuleFor(x => x.Dto.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0.");

            RuleFor(x => x.Dto.CourseId)
                .NotEmpty()
                .WithMessage("CourseId is required.");
        }
    }
}
