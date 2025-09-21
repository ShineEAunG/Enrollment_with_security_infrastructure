
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CourseEnrollment.Service.Implementations;

public class TokenService : ITokenService
{
    private readonly IEmployeeRepo _employeeRepo;
    private readonly IConfiguration _config;
    public TokenService(IEmployeeRepo employeeRepo, IConfiguration config)
    {
        this._employeeRepo = employeeRepo;
        this._config = config;
    }

    public string GenerateAccessToken(Employee employee)
    {
        // get the configured values from appsettings.json
        var issuer = _config["JwtSetting:Issuer"];
        var audience = _config["JwtSetting:Audience"];
        var secret = _config["JwtSetting:Secret"];
        var getAccessExpirationTime = _config["JwtSetting:AccessTokenExpiration"];
        if (secret is null || issuer is null || audience is null || getAccessExpirationTime is null)
            throw new ApplicationException("Jwt is not set in the configuration");

        // assign the expiration time
        var accessTokenExpireAt = DateTime.UtcNow.AddMinutes(int.Parse(getAccessExpirationTime));
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        // add the algorithm to the token
        var SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

        // list of claims => claim Identity => cliam principal
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, employee.EmployeeId.ToString()),
            new (ClaimTypes.Email, employee.Email)
        };
        var employeeRoles = employee.EmployeeRoles.Select(er=> er.Role.RoleName).ToList();
        foreach (var role in employeeRoles) // userRoles is a list of string
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }
        // ClaimsIdentity is the array of claims and is for creation of ClaimPrinciple(HttpContext.User etc) 
        var identity = new ClaimsIdentity(claims, "forJWT");

        // Token Descriptor => Token Handler => Create Token => Write Token
        var tokenDiscriptor = new SecurityTokenDescriptor   // Descriptor
        {
            Subject = identity,
            Issuer = issuer,
            Audience = audience,
            // this will not appear in jwt body payload but in header as algorithm
            SigningCredentials = SigningCredentials,    
            Expires = accessTokenExpireAt,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,            
        };
        var tokenHandler = new JwtSecurityTokenHandler();   // Handler
        var securityToken = tokenHandler.CreateToken(tokenDiscriptor);  // CreateToken()
        var token = tokenHandler.WriteToken(securityToken);             // WriteToken()
        return token;
        // var tokenOptions = new JwtSecurityToken(
        //     issuer: _config["Jwt:Issuer"],
        //     audience: _config["Jwt:Audience"],
        //     claims: claims,
        //     expires: DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:ExpiryInMinutes")),
        //     signingCredentials: creds
        // );
        // return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public string GenerateRefreshToken()
    {
        // here we can use it as randomize ulid or guid
        // but guid is 34 chars and got own structure and ulid is 24 
        // both got 128 bits (16 bytes) and are utf8 string not base 64 string
        // compare 128 vs 512 then the 512 will be huge problem to bruetforce
        const int tokenSize = 64;   // 512 bits
        var randomBytes = new byte[tokenSize];
        using (var rng = RandomNumberGenerator.Create())    // force to dispose at the end of {}
        {
            rng.GetBytes(randomBytes);    // fill randombytes variable with randomized binary numbers
        }
        // base64 => 4 chars = 3 bytes => 64*4/3 = 85 
        var rawToken = Convert.ToBase64String(randomBytes);
        return rawToken;    // 85 chars 
    }

    public string Sha256ComputeRefreshToken(string rawToken)    // Standard algorithm for jwt issuing
    {
        //make sure this cryptography is disposed
        using var sha256 = SHA256.Create();
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            string result = Convert.ToBase64String(bytes);
            return result;
        }
    }
}