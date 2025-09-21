
using System.Net.Http.Headers;
using CourseEnrollment.Models.Authentications;
using CourseEnrollment.Models.DTO;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository;
using CourseEnrollment.Service.Implementations;
using CourseEnrollment.Service.Interfaces;
using NuGet.Common;


namespace CourseEnrollment.Implementations;

public class AuthService : IAuthService
{
    private readonly RefreshTokenRepo _refreshRepo;
    private readonly IEmployeeRepo _employeeRepo;
    private readonly IConfiguration _config;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    public AuthService(RefreshTokenRepo refreshRepo, IEmployeeRepo employeeRepo, IConfiguration config, ITokenService tokenService, IEmailService emailService)
    {
        this._refreshRepo = refreshRepo;
        this._employeeRepo = employeeRepo;
        this._config = config;
        this._tokenService = tokenService;
        this._emailService = emailService;
    }

    public async Task<AuthResult> Login(LoginEmployeeDto loginEmployeeDto)
    {
        var employee = await _employeeRepo.GetEmployeeByEmail(loginEmployeeDto.Email);
        if (employee is not null)
        {
            bool checkPassword = PasswordHasherService
                .VerifyPassword(loginEmployeeDto.Password, employee.PasswordHash);    
            if (!checkPassword)
                return new AuthResult { Success = false };

            var newAuthResult = GetAuthResult(employee);
            var newRefreshToken = GetRefreshTokenToSaveInDb(newAuthResult, employee.EmployeeId);

            await _refreshRepo.AddRefreshToken(newRefreshToken);
            await _refreshRepo.SaveRefreshTokenInDb();
            return newAuthResult;
        }
        return new AuthResult { Success = false };
    }
    public RefreshToken GetRefreshTokenToSaveInDb(AuthResult newAuthResult, string employeeId)
    {
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = _tokenService.Sha256ComputeRefreshToken(newAuthResult.RefreshToken),
            CreatedAt = DateTime.UtcNow,
            EmployeeId = employeeId,
            IsActive = true,
            ExpiresAt = newAuthResult.RefreshTokenExpiresAt
        };
        return newRefreshToken;
    }
    public async Task<AuthResult> Register(RegisterEmployeeDto registerEmployeeDto)
    {
        var existedEmployee = await _employeeRepo.GetEmployeeByEmail(registerEmployeeDto.Email);
        if (existedEmployee is not null)
            return new AuthResult { Success = false };
        var newEmployee = new Employee
        {
            EmployeeId = Ulid.NewUlid().ToString(),
            EmployeeName = registerEmployeeDto.Name,
            Email = registerEmployeeDto.Email,
            PasswordHash = PasswordHasherService.HashPassword(registerEmployeeDto.Password),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = CreatedBy.System,
        };
        newEmployee = await _employeeRepo.Create(newEmployee);
        var addToRoleResult = await _employeeRepo.AddEmployeeToRoleAsync(newEmployee.EmployeeId, StaticEmployeeRole.Normal);
        // newEmployee = await _employeeRepo.GetEmployeeByEmail(newEmployee.Email);
        if (!addToRoleResult || newEmployee is null)
            return new AuthResult { Success = false };

        var otp = EmailGeneratorService.GenerateOtpCharacters();
        var content = EmailGeneratorService.GenerateEmailContent(otp);
        var subject = "Email Confirmation";
        var isEmailSent = await _emailService.SendEmailAsync(toEmail: newEmployee.Email, content: content, subject: subject);
        if (isEmailSent)
        {
            newEmployee.ConfirmationCode = otp;
            newEmployee.IsEmailConfirmed = false;
            await _employeeRepo.SaveChangesAsync();
            return new AuthResult { Success = true };
        }
            
        return new AuthResult { Success = false };
        
        // var newAuthResult =  GetAuthResult(newEmployee);
        //     var newRefreshToken = GetRefreshTokenToSaveInDb(newAuthResult, newEmployee.EmployeeId);
        //     await _refreshRepo.AddRefreshToken(newRefreshToken);
        //     await _refreshRepo.SaveRefreshTokenInDb();
        //     return newAuthResult;
    }
    public async Task<AuthResult> ConfirmEmail(string employeeEmail, string confirmationCode)
    {
        var employeeInDb = await _employeeRepo.GetEmployeeByEmail(employeeEmail);
        if (employeeInDb is null)
            return new AuthResult { Success = false };        

        if (confirmationCode == employeeInDb.ConfirmationCode)
        {
            employeeInDb.IsEmailConfirmed = true;
            await _employeeRepo.SaveChangesAsync();
            return GetAuthResult(employeeInDb);
        }
        return new AuthResult { Success = false };
    }
    public async Task<bool> Logout(string EmployeeId)
    {
        await _refreshRepo.RevokeRefreshToken(EmployeeId);
        return await _refreshRepo.SaveRefreshTokenInDb();
    }

    // here in refresh withoud cache there is still db read in every refresh but write is reduce by checking if the refresh token expiration is less than 1 day but the write to db is reduced much
    public async Task<AuthResult> Refresh(RefreshTokenRequest tokenDto)
    {
        var hashToken = _tokenService.Sha256ComputeRefreshToken(tokenDto.RefreshRawToken);
        var tokenInDb = await _refreshRepo.GetByHash(hashToken);
        if (tokenInDb is not null && tokenInDb.ExpiresAt > DateTime.UtcNow)
        {
            var employee = tokenInDb.Employee;
            if (employee is null)
                return new AuthResult { Success = false };

            var refreshTokenExpirationDay = tokenInDb.ExpiresAt.Subtract(DateTime.UtcNow).TotalDays;
            // if the refresh token is about to expire less than 1 day
            // rotate refresh token and write to db
            if (refreshTokenExpirationDay < 1)
            {
                var newAuthResultWithNewRefreshToken = GetAuthResult(employee);
                var newRefreshToken = GetRefreshTokenToSaveInDb(newAuthResultWithNewRefreshToken, employee.EmployeeId);
                tokenInDb.ReplacedByTokenId = newRefreshToken.Id;
                tokenInDb.RevokeAt = DateTime.UtcNow;
                tokenInDb.IsActive = false;
                await _refreshRepo.AddRefreshToken(newRefreshToken);
                await _refreshRepo.SaveRefreshTokenInDb();
                return newAuthResultWithNewRefreshToken;
            }
            else    // resue old refresh token
            {
                var newAccessToken = _tokenService.GenerateAccessToken(employee);
                var getAccessTokenExpiration = _config["JwtSetting:AccessTokenExpiration"];
                if (getAccessTokenExpiration is null)
                    throw new ApplicationException("jwt is not set in configuration");
                var newAuthResultWithOldRefreshToken = new AuthResult
                {
                    Success = true,
                    AccessToken = newAccessToken,
                    RefreshToken = tokenDto.RefreshRawToken,
                    AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(int.Parse(getAccessTokenExpiration)),
                    RefreshTokenExpiresAt = tokenInDb.ExpiresAt
                };
                return newAuthResultWithOldRefreshToken;
            }
        }
        return new AuthResult { Success = false };
    }
    

    private AuthResult GetAuthResult(Employee employee)
    {
        var getAccessTokenExpiration = _config["JwtSetting:AccessTokenExpiration"];
        var getRefreshTokenExpiration = _config["JwtSetting:RefreshTokenExpiration"];
        if (getAccessTokenExpiration is null || getRefreshTokenExpiration is null)
            throw new ApplicationException("Jwt is not set in the configuration");

        var accessTokenexpireAt = DateTime.UtcNow.AddMinutes(int.Parse(getAccessTokenExpiration));
        var refreshTokenexpireAt = DateTime.UtcNow.AddDays(int.Parse(getRefreshTokenExpiration));
        var authResult = new AuthResult
        {
            Success = true,
            AccessToken = _tokenService.GenerateAccessToken(employee),
            RefreshToken = _tokenService.GenerateRefreshToken(),
            AccessTokenExpireAt = accessTokenexpireAt,
            RefreshTokenExpiresAt = refreshTokenexpireAt
        };
        return authResult;
    }

    
}