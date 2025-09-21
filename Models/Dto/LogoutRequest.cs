using System.ComponentModel.DataAnnotations;

namespace CourseEnrollment.Models.DTO;

public class LogoutRequest
{
    [Required(ErrorMessage = "userId string is required")]
    public string UserId { get; set; } = null!;
}