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
                .Must(url =>
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                        return false;

                    return uri.Host.Equals("linkedin.com", StringComparison.OrdinalIgnoreCase) ||
                           uri.Host.Equals("www.linkedin.com", StringComparison.OrdinalIgnoreCase);
                })
                .WithMessage("Please enter a valid LinkedIn URL.");

            RuleFor(x => x.Dto.GitHubProfileUrl)
                .Must(url =>
                {
                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                        return false;

                    return uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
                           uri.Host.Equals("www.github.com", StringComparison.OrdinalIgnoreCase);
                })
                .WithMessage("Please enter a valid GitHub URL.");

            RuleFor(x => x.Dto.Bio)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(3000)
                .WithMessage("Description must not exceed 3000 characters.")
                .MinimumLength(50)
                .WithMessage("Description must be at least 50 characters long.");

            RuleFor(x => x.Dto.CvUrl)
                .NotNull()
                .WithMessage("CV is required.");

            RuleFor(x => x.Dto.CvUrl.ContentType)
                .Must(type => type is "application/pdf" or "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                .When(x => x.Dto.CvUrl != null)
                .WithMessage("Only PDF or DOCX files are allowed.");

            RuleFor(x => x.Dto.CvUrl.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .When(x => x.Dto.CvUrl != null)
                .WithMessage("File size must not exceed 5MB.");
        }
    }
}