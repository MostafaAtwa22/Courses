using FluentValidation;

namespace Application.Features.Discount.Commands.Update
{
    public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
    {
        public UpdateDiscountCommandValidator()
        {
            RuleFor(x => x.Dto.Percentage)
                .GreaterThan(0)
                .WithMessage("Percentage must be greater than zero.")
                .LessThanOrEqualTo(100)
                .WithMessage("Percentage must be less than or equal to 100.");
        }
    }
}
