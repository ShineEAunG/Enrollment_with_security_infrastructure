using CourseEnrollment.Models.ManyToMany;

namespace CourseEnrollment.Models.Authentications;


public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public bool IsActive => RevokeAt == null; // this is only c# code the column will not appear in db table
    public bool IsActive { get; set; }
    public DateTime? RevokeAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public string EmployeeId { get; set; } = null!;  // foreign key to retrieve user by id

    //make sure the foreign key configuration is there with right config if not the column name or others will be weird
    public Employee? Employee { get; set; } 
}