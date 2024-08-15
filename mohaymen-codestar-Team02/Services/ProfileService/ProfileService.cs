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

    public ProfileService(IHttpContextAccessor httpContextAccessor, DataContext context, ICookieService cookieService, IPasswordService passwordService)
    {
        _context = context;
        _cookieService = cookieService;
        _passwordService = passwordService;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }


    public async Task<ServiceResponse<string>> ChangePassword(string newPassword)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();

        var username = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(username))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
            return response;
        }

        var user = await GetUser(username);
        if (user is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        _passwordService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        response.Message = Resources.PasswordChangedSuccessfulyMessage;

        return response;
    }

    public ServiceResponse<string> Logout()
    {
        ServiceResponse<string> response = new ServiceResponse<string>();

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

        response.Type = ApiResponse.Success;
        response.Message = Resources.LogoutSuccessfuly;

        return response;
    }
}