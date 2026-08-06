using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkIncomplete;
using FluentValidation.TestHelper;

namespace Application.Tests.Progress.Validation
{
    public class MarkContentIncompleteCommandValidatorTests
    {
        private readonly MarkContentIncompleteCommandValidator _validator;

        public MarkContentIncompleteCommandValidatorTests()
        {
            _validator = new MarkContentIncompleteCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_ContentIdIsEmpty()
        {
            var command = new MarkContentIncompleteCommand(new MarkProgressRequestDto { ContentId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ContentId);
        }

        [Fact]
        public void Should_HaveError_When_CourseIdIsEmpty()
        {
            var command = new MarkContentIncompleteCommand(new MarkProgressRequestDto { CourseId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.CourseId);
        }

        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            var command = new MarkContentIncompleteCommand(new MarkProgressRequestDto 
            { 
                ContentId = Guid.NewGuid(), 
                CourseId = Guid.NewGuid() 
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
