using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public class AdminService : IAdminService
{
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;

    public AdminService(HttpContextAccessor httpContextAccessor, DataContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<ServiceResponse<string>> ChangeRole(User user, RoleType newRoleType)
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
            User user = _context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
            if (user is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = "User not found";
            } else if (!user.Roles.contains(RoleType.Admin))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = "access denied";
            }
            else
            {
                await _context.Users.UpdateAsync(); //
                await _context.SaveChangesAsync();

                response.Type = ApiResponse.Success; // redirect
            }
        }
        return response;
    }
 
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();
        var userId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];

        if (string.IsNullOrEmpty(userId))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = "Unauthorized";
        }
        else
        {
            User user = _context.Users.FirstOrDefault(x => x.Id.ToString() == userId);

            if (user is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = "User not found";
            } else if (!user.Roles.contains(RoleType.Admin))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = "access denied";
            }
            else
            {
                if (await UserExists(user.Username))
                {
                    response.Type = ApiResponse.Conflict;
                    response.Message = "User already exists";
                    return response;
                }
        
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt); //

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
        
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                response.Data = user.Id;
                response.Type = ApiResponse.Success; // redirect

                return response;
            }
        }
        return response;
    }
    
    public async Task<bool> UserExists(string username)
    {
        if (await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
        {
            return true;
        }

        return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512()) //
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
    
    

}