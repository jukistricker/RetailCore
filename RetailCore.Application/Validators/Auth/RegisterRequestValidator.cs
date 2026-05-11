namespace RetailCore.Application.Validators.Auth;


public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,100}$")
            .WithMessage("Password must be 8-100 characters and include uppercase, lowercase, number, and special character.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .Length(1, 100).WithMessage("Full name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z0-9 ]*$").WithMessage("Full name must contain only letters and numbers");
    }
}