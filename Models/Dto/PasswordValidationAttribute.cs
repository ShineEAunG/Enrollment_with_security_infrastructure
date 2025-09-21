using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models.DTO;


public class StrongPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? inputValue,ValidationContext validationContext)
    {
        var password = inputValue as string;    // assign as string

        if (string.IsNullOrWhiteSpace(password))    //check null or white spaces
            return new ValidationResult("password is required");

        bool valid = password.Length < 8;
        if (valid)
            return new ValidationResult("password must be longer than 8"); // check if it is longer

        if (!password.Any(char.IsUpper) || password.All(char.IsLetterOrDigit))  // check contains uppercase or all chars are letters or digits
            return new ValidationResult("password at least must be one upper and special character");
            

        return ValidationResult.Success!;   // default return
    }
}
