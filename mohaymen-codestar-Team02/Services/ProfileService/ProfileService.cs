using System.Linq;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public class ProfileService : IProfileService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<string>> ChangePassword(string username, long newPassword)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        var userId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];

        if (string.IsNullOrEmpty(userId))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = "Unauthorized";
        }
        else
        {
            User user = _context.Users.FirstOrDefault(x => x.UserId.ToString() == userId);

            if (user is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = "User not found";
            } else if (user.UserRoles.All(x => x.Role.RoleType != RoleType.SystemAdmin))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = "access denied";
            }
            else
            {
                User newUser = new User();
                CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.Salt = passwordSalt;
        
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
        return response;
    }

    public Task<ServiceResponse<string>> Logout(string username)
    {
        throw new NotImplementedException();
    }


    private void CreatePasswordHash(long password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512()) //
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password.ToString()));
        }
    }
}