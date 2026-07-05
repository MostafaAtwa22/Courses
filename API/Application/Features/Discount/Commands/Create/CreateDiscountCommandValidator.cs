using FluentValidation;

namespace Application.Features.Discount.Commands.Create
{
    public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
            RuleFor(x => x.Dto.Percentage)
                .GreaterThan(0)
                .WithMessage("Percentage must be greater than zero.")
                .LessThanOrEqualTo(100)
                .WithMessage("Percentage must be less than or equal to 100.");
        }
    }
}