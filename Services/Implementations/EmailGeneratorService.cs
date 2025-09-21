using System.Security.Cryptography;

namespace CourseEnrollment.Service.Implementations;

public static class EmailGeneratorService
{
    public static string GenerateOtpCharacters()
    {
        // var otp = new Random().Next(100000, 1000000); // numeric 6-digit
        // return otp;

        var size = 4;   // return 4-6 random charachters of base64
        var bytes32 = new byte[size];
        using (var rng = RandomNumberGenerator.Create())    //cryptographic randomness
        {
            rng.GetBytes(bytes32);
        }
        string otp = Convert.ToBase64String(bytes32);
        otp = otp.Replace('+', 'X')
            .Replace('/', 'Y')
            .Replace('=','z')
            .ToUpper(); // base 64 contains + and / but it is optional to replace
        // string otp2 = Encoding.UTF8.GetString(bytes32);
        return otp; // or otp2 but it can be hard to types coz the utf8 contains all the languages chars like chinese arabian or indian
    }
    public static string GenerateEmailContent(string otp)
    {
        var emailContent = $"Your confirmation code is \"{otp}\" please copy it and paste it in confirmation url";
        return emailContent;
    }
}