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
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public AdminService(IServiceProvider serviceProvider, ICookieService cookieService, ITokenService tokenService,
        IPasswordService passwordService, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _cookieService = cookieService;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<GetUserDto?>> GetUserByUsername(string? username)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId, context);
        if (admin is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        var user = await GetUser(username, context);
        if (user is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        var userDto = _mapper.Map<GetUserDto>(user);
        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success, Resources.UserRetrievedMassage);
    }

    public async Task<ServiceResponse<List<GetUserDto>?>> GetAllUsers()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<List<GetUserDto>?>(null, ApiResponseType.Unauthorized,
                Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId, context);
        if (admin is null)
            return new ServiceResponse<List<GetUserDto>?>(null, ApiResponseType.BadRequest,
                Resources.UserNotFoundMessage);

        var users = await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        var userDtos = users.Select(u => _mapper.Map<GetUserDto>(u)).ToList();
        return new ServiceResponse<List<GetUserDto>?>(userDtos, ApiResponseType.Success,
            Resources.UserRetrievedMassage);
    }

    public async Task<ServiceResponse<List<GetRoleDto>>> GetAllRoles()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var roles = await context.Roles.Select(r => _mapper.Map<GetRoleDto>(r)).ToListAsync();
        return new ServiceResponse<List<GetRoleDto>>(roles, ApiResponseType.Success, Resources.UsersRetrievedMassage);
    }

    public async Task<ServiceResponse<GetUserDto?>> Register(User user, string password, List<string> roles)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminUsername = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminUsername, context);
        if (admin is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        if (await UserExists(user.Username, context))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Conflict, Resources.UserAlreadyExistsMessage);

        if (!_passwordService.ValidatePassword(password))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest,
                Resources.YourPasswordIsNotValidated);

        if (!IsRoleMatching(roles, context))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest,
                Resources.SomeRolesAreInvalid);

        _passwordService.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
        user.PasswordHash = passwordHash;
        user.Salt = passwordSalt;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        foreach (var role in roles)
            await AddRole(
                new User { Username = user.Username },
                new Role() { RoleType = role }
            );

        var foundUser = await GetUser(user.Username, context);
        var userDto = _mapper.Map<GetUserDto>(foundUser);

        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Created,
            Resources.UserCreatedSuccessfullyMessage);
    }

    private bool IsRoleMatching(List<string> roles, DataContext context)
    {
        var matchingRoles = roles.All(r => context.Roles.Any(dbRole => dbRole.RoleType.ToLower() == r.ToLower()));
        return matchingRoles;
    }

    public async Task<ServiceResponse<GetUserDto?>> DeleteUser(User user)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId, context);
        if (admin is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        var foundUser = await GetUser(user.Username, context);
        if (foundUser is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        if (user.Username == admin.Username)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest,
                Resources.CanNotDeleteYourself);
        context.Users.Remove(foundUser);
        await context.SaveChangesAsync();

        var userDto = _mapper.Map<GetUserDto>(foundUser);
        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success, Resources.UserDeletionSuccessful);
    }

    public async Task<ServiceResponse<GetUserDto?>> AddRole(User user, Role role)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId, context);
        if (admin is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        var foundUser = await GetUser(user.Username, context);
        if (foundUser is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.RoleNotFoundMessage);

        if (await GetUserRole(foundRole, foundUser, context) is not null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.RoleAlreadyAssigned);

        var userRole = new UserRole
        {
            RoleId = foundRole.RoleId,
            UserId = foundUser.UserId
        };

        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        var userDto = _mapper.Map<GetUserDto>(foundUser);

        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success,
            Resources.RoleAddedSuccessfulyMassage);
    }

    public async Task<ServiceResponse<GetUserDto?>> DeleteRole(User user, Role role)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var token = _cookieService.GetCookieValue();
        if (string.IsNullOrEmpty(token))
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.Unauthorized, Resources.UnauthorizedMessage);

        var adminId = _tokenService.GetUserNameFromToken();
        var admin = await GetUser(adminId, context);
        if (admin is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.UserNotFoundMessage);

        var foundUser = await GetUser(user.Username, context);
        if (foundUser is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.NotFound, Resources.UserNotFoundMessage);

        var foundRole = await GetRole(role.RoleType);
        if (foundRole is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.RoleNotFoundMessage);


        var userRole = await GetUserRole(foundRole, foundUser, context);
        if (userRole is null)
            return new ServiceResponse<GetUserDto?>(null, ApiResponseType.BadRequest, Resources.DontHaveThisRole);

        context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync();

        var userDto = _mapper.Map<GetUserDto>(foundUser);

        return new ServiceResponse<GetUserDto?>(userDto, ApiResponseType.Success,
            Resources.RoleRemovedSuccessfullyMessage);
    }

    private async Task<User?> GetUser(string? username, DataContext dataContext)
    {
        return await dataContext.Users.FirstOrDefaultAsync(x =>
            username != null && x.Username != null && x.Username.ToLower().Equals(username.ToLower()));
    }

    private async Task<Role?> GetRole(string? roleType)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        if (roleType == null) return null;
        return await context.Roles.FirstOrDefaultAsync(x => x.RoleType.ToLower() == roleType.ToLower());
    }

    private async Task<UserRole?> GetUserRole(Role foundRole, User foundUser, DataContext dataContext)
    {
        return await dataContext.UserRoles.FirstOrDefaultAsync(x =>
            x.User.Username != null && foundUser.Username != null && x.RoleId == foundRole.RoleId &&
            x.User.Username.ToLower() == foundUser.Username.ToLower());
    }

    private async Task<bool> UserExists(string? username, DataContext dataContext)
    {
        return await dataContext.Users.AnyAsync(x =>
            username != null && x.Username != null && x.Username.ToLower() == username.ToLower());
    }
}