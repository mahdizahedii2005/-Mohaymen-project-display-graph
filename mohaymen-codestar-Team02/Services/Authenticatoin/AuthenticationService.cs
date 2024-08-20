using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService : IAuthenticationService
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;

    public AuthenticationService(DataContext context, ICookieService cookieService, ITokenService tokenService,
        IPasswordService passwordService)
    {
        _context = context;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<ServiceResponse<User>> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.InvalidInpute);
        var user = await GetUser(username);

        if (user is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!_passwordService.VerifyPasswordHash(password, user.PasswordHash, user.Salt))
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.WrongPasswordMessage);

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        _cookieService.CreateCookie(_tokenService.CreateToken(claims));

        return new ServiceResponse<User>(user, ApiResponseType.Success, Resources.LoginSuccessfulMessage);
    }


    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }
}