using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models.DTO;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh Token string is required")]
    public string RefreshRawToken { get; set; } = null!;
}