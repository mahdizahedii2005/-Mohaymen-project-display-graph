using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.Administration;

public class AdminService : IAdminService
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;

    public AdminService(DataContext context, ICookieService cookieService, ITokenService tokenService, IPasswordService passwordService)
    {
        _context = context;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }


    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
            return response;
        }

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId);
        if (admin is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        if (!await IsAdmin(admin))
        {
            response.Type = ApiResponse.Forbidden;
            response.Message = Resources.accessDeniedMessage;
            return response;
        }

        if (await UserExists(user.Username))
        {
            response.Type = ApiResponse.Conflict;
            response.Message = Resources.UserAlreadyExistsMessage;
            return response;
        }

        _passwordService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Created;
        response.Message = Resources.UserCreatedSuccessfullyMessage;
        return response;
    }


    public async Task<ServiceResponse<string>> AddRole(User user, Role role)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
            return response;
        }

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId);
        if (admin is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        if (!await IsAdmin(admin))
        {
            response.Type = ApiResponse.Forbidden;
            response.Message = Resources.accessDeniedMessage;
            return response;
        }

        var foundUser = await GetUser(user.Username);
        if (foundUser is null)
        {
            response.Type = ApiResponse.NotFound;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.RoleNotFoundMessage;
            return response;
        }

        if (await GetUserRole(foundRole, foundUser) is not null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.RoleAlreadyAssigned;
            return response;
        }

        UserRole userRole = new UserRole
        {
            Role = foundRole,
            User = foundUser,
            RoleId = foundRole.RoleId,
            UserId = foundUser.UserId
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        response.Message = Resources.RoleAddedSuccessfulyMassage;

        return response;
    }

    public async Task<ServiceResponse<string>> DeleteRole(User user, Role role)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
        {
            response.Type = ApiResponse.Unauthorized;
            response.Message = Resources.UnauthorizedMessage;
            return response;
        }

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId);
        if (admin is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        if (!await IsAdmin(admin))
        {
            response.Type = ApiResponse.Forbidden;
            response.Message = Resources.accessDeniedMessage;
            return response;
        }

        var foundUser = await GetUser(user.Username);
        if (foundUser is null)
        {
            response.Type = ApiResponse.NotFound;
            response.Message = Resources.UserNotFoundMessage;
            return response;
        }

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.RoleNotFoundMessage;
            return response;
        }


        var userRole = await GetUserRole(foundRole, foundUser);
        if (userRole is null)
        {
            response.Type = ApiResponse.BadRequest;
            response.Message = Resources.dontHaveThisRole;
        }

        else
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            response.Type = ApiResponse.Success;
            response.Message = Resources.RoleRemovedSuccessfullyMessage;
        }

        return response;
    }

    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }

    private async Task<bool> IsAdmin(User? user)
    {
        var role = GetRole(RoleType.SystemAdmin.ToString());
        return await _context.UserRoles.AnyAsync(x => user != null && x.UserId == user.UserId && x.RoleId == role.Id);
    }
    
    private async Task<Role?> GetRole(string roleType)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.RoleType.ToLower() == roleType.ToLower());
    }

    private async Task<UserRole?> GetUserRole(Role foundRole, User foundUser)
    {
        return await _context.UserRoles.FirstOrDefaultAsync(x =>
            x.RoleId == foundRole.RoleId && x.User.Username.ToLower() == foundUser.Username.ToLower());
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }
    
    
    

    // test
    public async Task<ServiceResponse<int>> RegisterUser(User user, string password)
    {
        ServiceResponse<int> response = new ServiceResponse<int>();

        _passwordService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt); //

        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        return response;
    }

    public async Task<ServiceResponse<string>> RegisterRoleTest(User user, string password)
    {
        ServiceResponse<string> response = new ServiceResponse<string>();

        Role role = new Role() { RoleType = RoleType.SystemAdmin.ToString(), RoleId = 2 };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        _passwordService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        UserRole userRole = new UserRole() { RoleId = role.RoleId, UserId = user.UserId, Role = role, User = user };
        _context.UserRoles.Add(userRole);
        user.UserRoles.Add(userRole);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        return response;
    }

    public async Task<ServiceResponse<string>> AddRoleTest()
    {
        ServiceResponse<string> response = new ServiceResponse<string>();
        Role role = new Role() { RoleType = RoleType.SystemAdmin.ToString() };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        response.Type = ApiResponse.Success;
        return response;
    }
}