
using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        RuleFor(temp => temp.ProductName).NotEmpty().WithMessage("Product name required")
        .Length(1, 50).WithMessage("Person Name should be 1 to 50 characters long");

        RuleFor(temp => temp.Category).IsInEnum().WithMessage("Available product categories are Electronics, HomeAppliances, Furniture");

        RuleFor(temp => temp.UnitPrice).InclusiveBetween(0, double.MaxValue).WithMessage($"Unit Price should between 0 to {double.MaxValue}");

        RuleFor(temp => temp.QuantityInStock).InclusiveBetween(0, int.MaxValue).WithMessage($"Unit Quantity should between 0 to {int.MaxValue}");
    }
}
