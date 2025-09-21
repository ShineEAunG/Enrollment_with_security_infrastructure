namespace CourseEnrollment.Models.ManyToMany;


public class Employee
{
    public string EmployeeId { get; set; } = Ulid.NewUlid().ToString();
    public string Email { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ConfirmationCode{ get; set; }
    public bool IsEmailConfirmed{ get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }    
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = [];
}

public class Role
{
    public string RoleId { get; set; } = Ulid.NewUlid().ToString();
    public string RoleName { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = [];
}
public class EmployeeRole
{
    public string EmployeeId { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public string RoleId { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

public static class StaticEmployeeRole
{
    public const string Admin = "admin";
    public const string Premium = "premium";
    public const string Normal = "normal";
}

public static class EmployeeClaimTypes
{
    public const string Role = "role";
    public const string Department = "department";
    public const string Email = "email";
    public const string Permission = "permission";
}

public static class CreatedBy
{
    public const string System = "system";
}