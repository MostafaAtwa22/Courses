using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkComplete;
using FluentValidation.TestHelper;

namespace Application.Tests.Progress.Validation
{
    public class MarkContentCompleteCommandValidatorTests
    {
        private readonly MarkContentCompleteCommandValidator _validator;

        public MarkContentCompleteCommandValidatorTests()
        {
            _validator = new MarkContentCompleteCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_ContentIdIsEmpty()
        {
            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto { ContentId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ContentId);
        }

        [Fact]
        public void Should_HaveError_When_CourseIdIsEmpty()
        {
            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto { CourseId = Guid.Empty });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.CourseId);
        }

        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto 
            { 
                ContentId = Guid.NewGuid(), 
                CourseId = Guid.NewGuid() 
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
