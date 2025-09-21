using System.Text;
using System.Text.Json.Serialization;
using CourseEnrollment.Data;
using CourseEnrollment.Implementations;
using CourseEnrollment.Models.ManyToMany;
using CourseEnrollment.Repository;
using CourseEnrollment.Repository.Implementation;
using CourseEnrollment.Repository.Interfaces;
using CourseEnrollment.Service;
using CourseEnrollment.Service.Implementations;
using CourseEnrollment.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CourseEnrollment.Extensions;

public static class ServiceCollectionExtensions
{

    public static void DbContextConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IStudentRepo, StudentRepo>();
        services.AddScoped<StudentService, StudentService>();
        services.AddScoped<IGenericRepo<Student>, GenericRepo<Student>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmployeeRepo, EmployeeRepo>();
        services.AddScoped<RefreshTokenRepo>();
        services.AddScoped<IEmailService, EmailService>();
    }
    public static void AddControllerConfig(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // to show enum string as output in api response, add this serializer converter
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                // to avoid circular reference issue when returning entity with navigation property
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
    }
    
    public static void JwtSchemeConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
            {
                // default scheme is cookies, here we change to jwt bearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // this only here is default
            }
        ).JwtBearerConfiguration(config);   //change to cookies or other scheme if needed
    }
    public static AuthenticationBuilder JwtBearerConfiguration(this AuthenticationBuilder authBuilder, IConfiguration config)
    {
        var issure = config["JwtSetting:Issuer"];
        var audience = config["JwtSetting:Audience"];
        var secret = config["JwtSetting:Secret"];

        if (secret is null || audience is null || issure is null)
            throw new ApplicationException("JwtSetting is not configured");

        var issureSingingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        authBuilder.AddJwtBearer(options =>
            {
                //options.RequireHttpsMetadata = false;  // only for development testing 
                // in production it is set to be true;
                options.SaveToken = true; // defalut is false if true save token in authentication properties and we can retrieve it later in service or controller
                // the line above is for this codes var token = await HttpContext.GetTokenAsync("access_token");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidIssuer = issure,
                    IssuerSigningKey = issureSingingKey
                };
            }
        );
        return authBuilder;
    }
    
}