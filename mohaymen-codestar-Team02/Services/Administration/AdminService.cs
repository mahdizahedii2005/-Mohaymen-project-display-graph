using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using ApiResponseType = mohaymen_codestar_Team02.Models.ApiResponseType;

namespace mohaymen_codestar_Team02.Services.Administration;

public class AdminService : IAdminService
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public AdminService(DataContext context, ICookieService cookieService, ITokenService tokenService,
        IPasswordService passwordService, IMapper mapper)
    {
        _context = context;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto>> GetUserByUsername(string username)
    {
        var user = await GetUser(username);
        var userDto = _mapper.Map<GetUserDto>(user);
        return new ServiceResponse<GetUserDto>(userDto, ApiResponseType.Success, "");
    }

    public async Task<ServiceResponse<List<GetUserDto>>> GetAllUsers()
    {
        List<User> users = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        List<GetUserDto> userDtos = users.Select(u => _mapper.Map<GetUserDto>(u)).ToList();
        return new ServiceResponse<List<GetUserDto>>(userDtos, ApiResponseType.Success, "");
    }

    public async Task<ServiceResponse<List<GetRoleDto>>> GetAllRoles()
    {
        List<GetRoleDto> roles = await _context.Roles.Select(r => _mapper.Map<GetRoleDto>(r)).ToListAsync();
        return new ServiceResponse<List<GetRoleDto>>(roles, ApiResponseType.Success, "");
    }

    public async Task<ServiceResponse<User>> Register(User user, string password)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<User>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminUsername = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminUsername);
        if (admin is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!await IsAdmin(admin))
            return new ServiceResponse<User>(null, ApiResponseType.Forbidden, Resources.accessDeniedMessage);

        if (await UserExists(user.Username))
            return new ServiceResponse<User>(null, ApiResponseType.Conflict, Resources.UserAlreadyExistsMessage);

        _passwordService.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new ServiceResponse<User>(user, ApiResponseType.Created, Resources.UserCreatedSuccessfullyMessage);
    }


    public async Task<ServiceResponse<User>> AddRole(User user, Role role)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<User>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId);
        if (admin is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!await IsAdmin(admin))
            return new ServiceResponse<User>(null, ApiResponseType.Forbidden, Resources.accessDeniedMessage);

        var foundUser = await GetUser(user.Username);
        if (foundUser is null)
            return new ServiceResponse<User>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.RoleNotFoundMessage);

        if (await GetUserRole(foundRole, foundUser) is not null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.RoleAlreadyAssigned);

        var userRole = new UserRole
        {
            Role = foundRole,
            User = foundUser,
            RoleId = foundRole.RoleId,
            UserId = foundUser.UserId
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        return new ServiceResponse<User>(user, ApiResponseType.Success, Resources.RoleAddedSuccessfulyMassage);
    }

    public async Task<ServiceResponse<User>> DeleteRole(User user, Role role)
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<User>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId);
        if (admin is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!await IsAdmin(admin))
            return new ServiceResponse<User>(null, ApiResponseType.Forbidden, Resources.accessDeniedMessage);

        var foundUser = await GetUser(user.Username);
        if (foundUser is null)
            return new ServiceResponse<User>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.RoleNotFoundMessage);


        var userRole = await GetUserRole(foundRole, foundUser);
        if (userRole is null)
            return new ServiceResponse<User>(null, ApiResponseType.BadRequest, Resources.dontHaveThisRole);

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return new ServiceResponse<User>(foundUser, ApiResponseType.Success, Resources.RoleRemovedSuccessfullyMessage);
    }

    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }

    private async Task<bool> IsAdmin(User? user)
    {
        if (user == null) return false;
        var targetUSer = _context.Users.SingleOrDefault(a => a.Username == user.Username);
        var result = targetUSer.UserRoles.Any(Userrole =>
            Userrole.Role.RoleType.ToLower() == RoleType.SystemAdmin.ToString().ToLower());
        return result;
    }

    private async Task<Role?> GetRole(string? roleType)
    {
        if (roleType == null) return null;
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
}