using FluentValidation;
using RetailCore.Shared.DTOs;

namespace RetailCore.Shared.Validators.Product;

public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be less than 1000 characters");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .InclusiveBetween(1, 1000000).WithMessage("Price must be between 1 and 1000000$");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Length(3, 100).WithMessage("Slug must be between 3 and 100 characters")
            .Matches("^[a-z0-9-]+$").WithMessage("Slug must be lowercase, numbers, and dashes");

        RuleFor(x => x.Stock)
            .NotEmpty().WithMessage("Stock is required")
            .InclusiveBetween(1, 1000000).WithMessage("Stock must be between 1 and 1000000");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Images)
            .NotEmpty().WithMessage("Images is required");
    }
}