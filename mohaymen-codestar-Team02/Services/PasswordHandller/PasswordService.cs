using System.Text.RegularExpressions;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.PasswordHandller;

public class PasswordService : IPasswordService
{
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            for (var i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != passwordHash[i])
                    return false;

            return true;
        }
    }

    public bool ValidatePassword(string password)
    {
        const string PasswordPattern = CustomRegexPattern.PasswordPattern;
        var regex = new Regex(PasswordPattern);
        return regex.IsMatch(password);
    }
}