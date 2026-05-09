namespace RetailCore.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [Range(8, 100, ErrorMessage = "Password must be between 8 and 100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Full name is required")]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Full name must contain only letters and numbers" )]
    [Range(1,100, ErrorMessage = "Full name must be between 1 and 100 characters")]
    public string FullName { get; set; }
}