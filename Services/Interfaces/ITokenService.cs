using CourseEnrollment.Models.ManyToMany;

namespace CourseEnrollment.Service.Interfaces;

public interface ITokenService
{
    string GenerateRefreshToken();
    string Sha256ComputeRefreshToken(string rawToken);
    string GenerateAccessToken(Employee employee);
}