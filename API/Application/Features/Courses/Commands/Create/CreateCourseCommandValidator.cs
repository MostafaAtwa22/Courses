using FluentValidation;

namespace Application.Features.Courses.Commands.Create
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(200)
                .WithMessage("Name must not exceed 100 characters.")
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters long.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(3000)
                .WithMessage("Description must not exceed 3000 characters.")
                .MinimumLength(50)
                .WithMessage("Description must be at least 50 characters long.");

            RuleFor(x => x.Dto.Status)
                .IsInEnum()
                .WithMessage("The Status must be in (In Progress - Completed)");

            RuleFor(x => x.Dto.PictureUrl)
                .NotNull()
                .WithMessage("Picture is required.");

            RuleFor(x => x.Dto.PictureUrl.ContentType)
                .Must(type => type is "image/jpeg" or "image/png" or "image/webp")
                .When(x => x.Dto.PictureUrl != null)
                .WithMessage("Only JPEG, PNG, or WebP images are allowed.");

            RuleFor(x => x.Dto.PictureUrl.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .When(x => x.Dto.PictureUrl != null)
                .WithMessage("Image size must not exceed 5MB.");
        }
    }
}