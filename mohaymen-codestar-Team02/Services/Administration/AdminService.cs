using System.Drawing;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public class AdminService : IAdminService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _context;

    public AdminService(IHttpContextAccessor httpContextAccessor, DataContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    
    public async Task<ServiceResponse<string>> Regiser2(User user, long password)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        Role role = new Role() {RoleType = RoleType.SystemAdmin, RoleId = 2};
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        UserRole userRole = new UserRole() {RoleId = role.RoleId, UserId = user.UserId, Role = role, User = user};
        _context.UserRoles.Add(userRole);
        user.UserRoles.Add(userRole);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        return response;
    }

    
    public async Task<ServiceResponse<string>> AddRole(User user, int roleId)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        var adminId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];

        if (string.IsNullOrEmpty(adminId))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
        }
        else
        {
            User admin = _context.Users.FirstOrDefault(x => x.UserId.ToString() == adminId);

            if (admin is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = Resources.UserNotFoundMessage;
            }
            else if (!admin.UserRoles.Any(x => x.Role.RoleType.Equals(RoleType.SystemAdmin)))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = Resources.accessDeniedMessage;
            }
            else
            {
                Role role = _context.Roles.FirstOrDefault(x => x.RoleId == roleId);

                user.UserRoles.Add(new UserRole() { Role = role, User = user, RoleId = roleId, UserId = user.UserId });

                await _context.SaveChangesAsync();

                response.Type = ApiResponse.Success; // redirect
            }
        }

        return response;
    }


    public async Task<ServiceResponse<string>> DeleteRole(User user, int roleId)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        var userId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];

        if (string.IsNullOrEmpty(userId))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
        }
        else
        {
            User admin = _context.Users.FirstOrDefault(x => x.UserId.ToString() == userId);

            if (admin is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = Resources.UserNotFoundMessage;
            }
            else if (!admin.UserRoles.Any(x => x.Role.RoleType.Equals(RoleType.SystemAdmin)))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = Resources.accessDeniedMessage;
            }
            else
            {
                var userRoleole = user.UserRoles.FirstOrDefault(x => x.RoleId == roleId);

                if (userRoleole != null)
                {
                    user.UserRoles.Remove(userRoleole);

                    await _context.SaveChangesAsync();

                    response.Type = ApiResponse.Success; // redirect
                }
                else
                {
                    response.Type = ApiResponse.NotFound;
                    response.Message = Resources.UserNotFoundMessage;
                }
            }
        }

        return response;
    }

    public async Task<ServiceResponse<int>> Register1(User user, long password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt); //

        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        return response;
    }

    
    public async Task<ServiceResponse<int>> Register(User user, long password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();
        var adminId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];

        if (string.IsNullOrEmpty(adminId))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
        }
        else
        {
            User admin = _context.Users.FirstOrDefault(x => x.UserId.ToString() == adminId);

            if (admin is null)
            {
                response.Type = ApiResponse.BadRequest;
                response.Message = Resources.UserNotFoundMessage;
            }
            else if(!_context.UserRoles.Any(x=>(x.UserId==admin.UserId && x.RoleId==2)))
            {
                response.Type = ApiResponse.Forbidden;
                response.Message = Resources.accessDeniedMessage;
            }
            else
            {
                if (await UserExists(user.Username))
                {
                    response.Type = ApiResponse.Conflict;
                    response.Message = Resources.UserAlreadyExistsMessage;
                    return response;
                }

                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt); //

                user.PasswordHash = passwordHash;
                user.Salt = passwordSalt;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                response.Type = ApiResponse.Created; // redirect

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

    private void CreatePasswordHash(long password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password.ToString()));
        }
    }
}