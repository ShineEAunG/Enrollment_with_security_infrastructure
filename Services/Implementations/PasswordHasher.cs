using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace CourseEnrollment.Service.Implementations;

// password hashing => randomization + "." + hashed input as hashed password
public static class PasswordHasherService
{
    private static (byte[],byte[]) PasswordBasedKeyDerivationFun2(string password,byte[]? randomBytes)
    {
        const int size = 128 / 8;     // 16 bytes
        if (randomBytes is null)
        {
            randomBytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();    // force to dispose at the end of {}
            rng.GetBytes(randomBytes);                          // fill randombytes variable with randomized binary numbers
        }
        var keyDerivationPrf = KeyDerivationPrf.HMACSHA256; // Pseudo-Random Function(Prf) is algorithm to pass to f2 
                                                            // not equivalent to SecurityAlgorithms.HmacSha256Signature
        var iterationCount = 100000;         // recommanded ammount
        var numBytesRequested = 256 / 8;    // 32bytes
        var passwordBasedKeyDerivation = KeyDerivation.Pbkdf2(password,  randomBytes, keyDerivationPrf, iterationCount, numBytesRequested);
        return (passwordBasedKeyDerivation, randomBytes);
    }
    public static string HashPassword(string password)
    {
        var passwordBasedKeyDerivation = PasswordBasedKeyDerivationFun2(password, null);
        var randomBytes = passwordBasedKeyDerivation.Item2;

        var hashedPassword = $"{Convert.ToBase64String(randomBytes)}.{Convert.ToBase64String(passwordBasedKeyDerivation.Item1)}";
        return hashedPassword;
    }

    public static bool VerifyPassword(string password, string passwordHash)
    {
        var split = passwordHash.Split('.');    // split the hashed passowrd
        if (split.Length != 2)
            return false;
        var hashedBytesInDb = Convert.FromBase64String(split[1]);   //from string to bytes
        var randomString = split[0];
        var randomBytes = Convert.FromBase64String(randomString);   //from string to bytes
        var passwordBasedKeyDerivation = PasswordBasedKeyDerivationFun2(password,randomBytes);  //get tuples 
        var hashedPassword = passwordBasedKeyDerivation.Item1;
        // return Convert.ToBase64String(hashedBytesInDb) == split[1]; // string camparison
        return CryptographicOperations.FixedTimeEquals(hashedPassword, hashedBytesInDb);    //safer than string comparison on timing attacks
    }
}