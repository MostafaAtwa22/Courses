using FluentValidation;

namespace Application.Features.Instructors.Commands.Update
{
    public class UpdateInstructorCommandValidator : AbstractValidator<UpdateInstructorCommand>
    {
        public UpdateInstructorCommandValidator()
        {
        }
    }
}