using CourseEnrollment.Models.DTO;

namespace CourseEnrollment.Service.Interfaces;

public interface IAuthService
{
    Task<AuthResult> Register(RegisterEmployeeDto registerEmployeeDto);
    Task<AuthResult> Login(LoginEmployeeDto loginEmployeeDto);
    Task<bool> Logout(string EmployeeId);
    Task<AuthResult> Refresh(RefreshTokenRequest tokenDto);
    Task<AuthResult> ConfirmEmail(string employeeEmail, string confirmationCode);
}