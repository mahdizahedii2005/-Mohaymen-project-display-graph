using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService : IAuthenticationService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthenticationService( IHttpContextAccessor httpContextAccessor,DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

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
            CreateCookie(user.UserId.ToString()); 
           
            response.Type = ApiResponse.Success;
            response.Message = Resources.LoginSuccessfulMessage;
        }

        return response;
    }

    private void CreateCookie(string data) //
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(1)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("login", data, cookieOptions);
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // role
            new Claim(ClaimTypes.Name, user.Username)
        };

        SymmetricSecurityKey key =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }

    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
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