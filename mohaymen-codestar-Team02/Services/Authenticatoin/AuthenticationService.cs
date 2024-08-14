using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService(DataContext context, ICookieService cookieService, ITokenService tokenService)
    : IAuthenticationService
{
    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        var user = await GetUser(username);

        if (user is null)
        {
            response.Type = ApiResponse.NotFound;
            response.Message = Resources.UserNotFoundMessage;
            response.Data = "Data";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.Salt))
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.WrongPasswordMessage;
            response.Data = "Data";
        }
        else
        {
            cookieService.CreateCookie(tokenService.CreateToken(user));
            
            response.Type = ApiResponse.Success;
            response.Message = Resources.LoginSuccessfulMessage;
        }

        return response;
    }
    
    

    private async Task<User?> GetUser(string username)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}