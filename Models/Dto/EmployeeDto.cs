using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models.DTO;

public class RegisterEmployeeDto
{
    // this data annotation will available in runtime
    [Required(ErrorMessage = "User name is required")]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [StrongPassword]    // here check if the password got special or upper at least one
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
    public string? ConfirmPassword { get; set; }
}


public class LoginEmployeeDto
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } =null!;
}