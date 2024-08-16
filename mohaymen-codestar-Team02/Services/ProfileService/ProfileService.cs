using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public class ProfileService : IProfileService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public ProfileService(IHttpContextAccessor httpContextAccessor, DataContext context, ICookieService cookieService,
        IPasswordService passwordService, ITokenService tokenService)
    {
        _context = context;
        _cookieService = cookieService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<User>> ChangePassword(string newPassword)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            return new ServiceResponse<User>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);
        }

        var username = _tokenService.GetUserNameFromToken();
        var user = await GetUser(username);
        if (user is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        _passwordService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new ServiceResponse<User>(user, ApiResponseType.Success, Resources.PasswordChangedSuccessfulyMessage);
    }

    public ServiceResponse<User> Logout()
    {
        if (_httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey("login") == true)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(-1)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("login", "", cookieOptions);
        }

        return new ServiceResponse<User>(null, ApiResponseType.Success, Resources.LogoutSuccessfuly);
    }

    public async Task<ServiceResponse<User>> UpdateUser(User newUser)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            return new ServiceResponse<User>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);
        }

        var username = _tokenService.GetUserNameFromToken();
        var user = await GetUser(username);
        if (user is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        user.FirstName = newUser.FirstName;
        user.LastName = newUser.LastName;
        user.Email = newUser.Email;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new ServiceResponse<User>(user, ApiResponseType.Success, Resources.ProfileInfoUpdateSuccessfulyMessage);
    }

    private Task<User?> GetUser(string username) =>
        _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
}