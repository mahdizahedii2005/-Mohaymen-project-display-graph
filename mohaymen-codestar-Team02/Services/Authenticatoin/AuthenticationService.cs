using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService : IAuthenticationService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthenticationService( IHttpContextAccessor httpContextAccessor,DataContext context)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<string>> Login(string username, long password)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        User user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

        if (user is null)
        {
            response.Type = ApiResponse.NotFound;
            response.Message = Resources.UserNotFoundMessage;
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.Salt))
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.WrongPasswordMessage;
        }
        else
        {
            string userId = user.UserId.ToString();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddHours(1)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("userId", userId, cookieOptions);

            response.Type = ApiResponse.Success;
            response.Message = Resources.LoginSuccessfulMessage;
            // response.Message = "successful";
        }

        return response;
    }

    private bool VerifyPasswordHash(long password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password.ToString()));

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