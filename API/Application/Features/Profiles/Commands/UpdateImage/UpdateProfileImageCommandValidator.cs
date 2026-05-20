using FluentValidation;

namespace Application.Features.Profiles.Commands.UpdateImage
{
    public class UpdateProfileImageCommandValidator : AbstractValidator<UpdateProfileImageCommand>
    {
        public UpdateProfileImageCommandValidator()
        {
            RuleFor(x => x.Dto.Image)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => file != null && file.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size must not exceed 5 MB.")
                .Must(file =>
                {
                    if (file == null) return false;
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp";
                })
                .WithMessage("Only .jpg, .jpeg, .png, and .webp file extensions are allowed.")
                .Must(file => file != null && file.ContentType.StartsWith("image/"))
                .WithMessage("File must be an image type.");
        }
    }
}
