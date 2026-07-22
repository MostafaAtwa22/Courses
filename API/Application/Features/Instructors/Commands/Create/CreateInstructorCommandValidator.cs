using FluentValidation;

namespace Application.Features.Instructors.Commands.Create
{
    public class CreateInstructorCommandValidator : AbstractValidator<CreateInstructorCommand>
    {
        public CreateInstructorCommandValidator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 100 characters.")
                .MinimumLength(3)
                .WithMessage("Title must be at least 3 characters long.");

            RuleFor(x => x.Dto.LinkedInProfileUrl)
                .NotEmpty()
                .WithMessage("LinkedIn Profile Url is required.")
                .MaximumLength(200)
                .WithMessage("LinkedIn Profile Url must not exceed 100 characters.")
                .MinimumLength(50)
                .WithMessage("LinkedIn Profile Url must be at least 50 characters long.");

            RuleFor(x => x.Dto.GitHubProfileUrl)
                .NotEmpty()
                .WithMessage("Github Profile Url is required.")
                .MaximumLength(200)
                .WithMessage("Github Profile Url must not exceed 100 characters.")
                .MinimumLength(50)
                .WithMessage("Github Profile Url must be at least 50 characters long.");

            RuleFor(x => x.Dto.Bio)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(3000)
                .WithMessage("Description must not exceed 3000 characters.")
                .MinimumLength(50)
                .WithMessage("Description must be at least 50 characters long.");
            
            RuleFor(x => x.Dto.CvUrl)
                .NotNull()
                .WithMessage("Picture is required.");

            RuleFor(x => x.Dto.CvUrl.ContentType)
                .Must(type => type is "image/pdf" or "image/do" or "image/webp")
                .When(x => x.Dto.CvUrl != null)
                .WithMessage("Only JPEG, PNG, or WebP images are allowed.");

            RuleFor(x => x.Dto.CvUrl.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .When(x => x.Dto.CvUrl != null)
                .WithMessage("Image size must not exceed 5MB.");
        }
    }
}