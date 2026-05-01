using Application.DTOs.Review;
using Application.Features.Reviews.Commands.Create;
using FluentValidation.TestHelper;

namespace Application.Tests.Reviews.Validation
{
    public class CreateReviewCommandValidatorTests
    {
        private readonly CreateReviewCommandValidator _validator;

        public CreateReviewCommandValidatorTests()
        {
            _validator = new CreateReviewCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_HeadlineIsEmpty()
        {
            var command = new CreateReviewCommand(new ReviewCreateDto { Headline = "" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.Headline);
        }

        [Fact]
        public void Should_HaveError_When_RatingIsOutOfRange()
        {
            var command = new CreateReviewCommand(new ReviewCreateDto { Rating = 6 });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.Rating);
        }

        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            var command = new CreateReviewCommand(new ReviewCreateDto 
            { 
                Headline = "Valid Headline", 
                Comment = "Valid Comment", 
                Rating = 5, 
                CourseId = Guid.NewGuid() 
            });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
