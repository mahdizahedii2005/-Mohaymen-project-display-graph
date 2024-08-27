using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.Permission;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public class AuthenticationService : IAuthenticationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public AuthenticationService(IServiceProvider serviceProvider, ICookieService cookieService,
        ITokenService tokenService,
        IPasswordService passwordService, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto?>> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.InvalidInputeMessage);

        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var user = await GetUser(username, dataContext);

        if (user is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (!_passwordService.VerifyPasswordHash(password, user.PasswordHash, user.Salt))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.WrongPasswordMessage);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.Username)
        };

        claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.RoleType)));

        _cookieService.CreateCookie(_tokenService.CreateToken(claims));

        var userDto = _mapper.Map<GetUserDto>(user);

        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success, Resources.LoginSuccessfulMessage);
    }

    public ServiceResponse<string?> Logout()
    {
        if (_cookieService.GetCookieValue() != null) _cookieService.GetExpiredCookie();

        return new ServiceResponse<string?>(null, ApiResponseType.Success,
            Resources.LogoutSuccessfulyMessage);
    }

    public async Task<ServiceResponse<GetPermissionDto>> GetPermission()
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetPermissionDto>(null, ApiResponseType.Unauthorized,
                Resources.UnauthorizedMessage);

        var username = _tokenService.GetUserNameFromToken();

        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var user = await GetUser(username, dataContext);
        if (user is null)
            return new ServiceResponse<GetPermissionDto>(null, ApiResponseType.BadRequest,
                Resources.UserNotFoundMessage);

        var roles = _tokenService.GetRolesFromToken();
        var splitRoles = roles?.Split(",");

        var permissions = await UnionPermissions(splitRoles);

        var permissionDto = new GetPermissionDto()
        {
            Permissions = permissions.ToList()
        };

        return new ServiceResponse<GetPermissionDto>(permissionDto, ApiResponseType.Success,
            Resources.GetPermissionsSuccessfulyMessage);
    }

    public async Task<ServiceResponse<string>> GetAuthorized()
    {
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<string>(null, ApiResponseType.Unauthorized,
                Resources.UnauthorizedMessage);

        var username = _tokenService.GetUserNameFromToken();

        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var user = await GetUser(username, dataContext);

        if (user is null)
            return new ServiceResponse<string>(null, ApiResponseType.BadRequest,
                Resources.UserNotFoundMessage);

        return new ServiceResponse<string>(user.Username, ApiResponseType.Success,
            Resources.AuthorizedMessage);
    }

    private async Task<HashSet<Permission>> UnionPermissions(string[]? splitRoles)
    {
        using var scope = _serviceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var permissions = new HashSet<Permission>();
        foreach (var userRole in splitRoles)
        {
            var role = await dataContext.Roles.FirstOrDefaultAsync(r => r.RoleType.ToLower() == userRole.ToLower());
            var permission = role?.Permissions;

            if (permission == null) continue;
            permissions.UnionWith(permission);
        }

        return permissions;
    }


    private async Task<User?> GetUser(string username, DataContext dataContext)
    {
        return await dataContext.Users.FirstOrDefaultAsync(x =>
            x.Username != null && x.Username.ToLower().Equals(username.ToLower()));
    }
}