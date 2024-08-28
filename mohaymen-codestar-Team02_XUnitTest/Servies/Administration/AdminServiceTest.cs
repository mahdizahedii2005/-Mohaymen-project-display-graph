using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.Administration;

public class AdminServiceTest
{
    private readonly AdminService _sut;
    private readonly ICookieService _cookieService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public AdminServiceTest()
    {
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();
        _passwordService = Substitute.For<IPasswordService>();
        _mapper = Substitute.For<IMapper>();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => new DataContext(options));
        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new AdminService(_serviceProvider, _cookieService, _tokenService, _passwordService, _mapper);
    }

    [Fact]
    public async Task Register_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.CreateUser(null, "password", new List<string>());

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.CreateUser(null, "password", new List<string>());

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUserAlreadyExists()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var existingUser = AddUserWithRole("existingUser", "Analyst", 2, mockContext);

        // Act
        var result = await _sut.CreateUser(existingUser.User, "password", new List<string>());

        // Assert
        Assert.Equal(ApiResponseType.Conflict, result.Type);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenUserIsSuccessfullyRegistered()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        var newUser = AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var fakePasswordHash = new byte[] { 1, 2, 3, 4 };
        var fakePasswordSalt = new byte[] { 5, 6, 7, 8 };

        _passwordService
            .When(x => x.CreatePasswordHash(Arg.Any<string>(), out Arg.Any<byte[]>(), out Arg.Any<byte[]>()))
            .Do(x =>
            {
                x[1] = fakePasswordHash;
                x[2] = fakePasswordSalt;
            });

        var role = new Role { RoleType = RoleType.DataAdmin.ToString() };
        mockContext.Roles.Add(role);
        mockContext.SaveChanges();

        _passwordService.ValidatePassword(Arg.Any<string>()).Returns(true);

        // Act
        var result = await _sut.CreateUser(
            new User() { UserId = 8, Username = "mamad" }, "password", new List<string>() { "DataAdmin" });

        // Assert
        Assert.Equal(ApiResponseType.Created, result.Type);
    }


    [Fact]
    public async Task Register_ShouldSetCorrectPasswordHashAndSalt_WhenUserIsSuccessfullyRegistered()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);

        var fakePasswordHash = new byte[] { 1, 2, 3, 4 };
        var fakePasswordSalt = new byte[] { 5, 6, 7, 8 };

        _passwordService
            .When(x => x.CreatePasswordHash(Arg.Any<string>(), out Arg.Any<byte[]>(), out Arg.Any<byte[]>()))
            .Do(x =>
            {
                x[1] = fakePasswordHash;
                x[2] = fakePasswordSalt;
            });
        _passwordService.ValidatePassword(Arg.Any<string>()).Returns(true);
        var newUser = new User() { UserId = 8, Username = "mamad" };

        // Act
        var result = await _sut.CreateUser(newUser, "password", new List<string>());

        // Assert
        Assert.Equal(ApiResponseType.Created, result.Type);
        Assert.Equal(fakePasswordHash, newUser.PasswordHash);
        Assert.Equal(fakePasswordSalt, newUser.Salt);
    }


    [Fact]
    public async Task GetUserByUsername_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.GetUserByUsername(null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnSuccess_WhenUserIsFound()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        var adminUser = AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var testUser = AddUserWithRole("testUser", "SystemAdmin", 2, mockContext);
        var userDto = new GetUserDto { Username = "testUser" };
        _mapper.Map<GetUserDto>(Arg.Any<User>()).Returns(userDto);

        // Act
        var result = await _sut.GetUserByUsername("testUser");

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.NotNull(result.Data);
        Assert.Equal("testUser", result.Data.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.GetUserByUsername("testUser");

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        var adminUser = AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var targetUser = AddUserWithRole("targetUser", "SystemAdmin", 2, mockContext);
        var userDto = new GetUserDto { Username = "targetUser" };
        _mapper.Map<GetUserDto>(Arg.Any<User>()).Returns(userDto);

        // Act
        var result = await _sut.GetUserByUsername("testUser");

        // Assert
        Assert.Equal(ApiResponseType.NotFound, result.Type);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.GetUsersPaginated(1);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsersSuccessfully_WhenAdminExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        var adminUser = AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        AddUserWithRole("user1", "User", 2, mockContext);
        AddUserWithRole("user2", "User", 3, mockContext);

        _mapper.Map<GetUserDto>(Arg.Any<User>()).Returns(new GetUserDto());

        // Act
        var result = await _sut.GetUsersPaginated(1);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.GetUsersPaginated(1);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnAllRolesSuccessfully()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        mockContext.Roles.Add(new Role { RoleId = 1, RoleType = "SystemAdmin" });
        mockContext.Roles.Add(new Role { RoleId = 2, RoleType = "Analyst" });
        await mockContext.SaveChangesAsync();

        _mapper.Map<GetRoleDto>(Arg.Any<Role>()).Returns(new GetRoleDto());

        // Act
        var result = await _sut.GetAllRoles();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Equal(2, result.Data.Count);
    }

    private void FixTheReturnOfCookies(string? returnThis)
    {
        _cookieService.GetCookieValue().Returns(returnThis);
        _tokenService.GetUserNameFromToken().Returns(returnThis);
    }

    private UserRole AddUserWithRole(string userName, string roleType, long id, DataContext dataContext)
    {
        var user = new User
        {
            Salt = Array.Empty<byte>(),
            PasswordHash = Array.Empty<byte>(),
            Username = userName,
            UserId = id
        };
        var role = new Role { RoleType = roleType, RoleId = id };
        var userRole = new UserRole { UserId = user.UserId, RoleId = role.RoleId };
        dataContext.Users.Add(user);
        dataContext.Roles.Add(role);
        dataContext.UserRoles.Add(userRole);
        dataContext.SaveChanges();
        return new UserRole { Role = role, User = user };
    }

    [Fact]
    public async Task AddRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnSuccess_WhenUserAndAdminExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);
        var role = AddUserWithRole("fakeUser", "Analyst", 3, mockContext);

        // Act
        var result = await _sut.AddRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.AddRole(new User() { Username = "testUser" },
            new Role() { RoleType = RoleType.DataAdmin.ToString() });
        // Assert
        Assert.Equal(ApiResponseType.NotFound, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenRoleIsNotFound()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.AddRole(user.User, new Role() { RoleType = "testRole" });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenRoleAlreadyAssigned()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.AddRole(user.User, new Role() { RoleType = RoleType.DataAdmin.ToString() });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleNotExists()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        AddUserWithRole("lol", "SystemAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteRole(new User { Username = "lol" }, new Role() { RoleType = "invalidRole" });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleExistsAndUserDontHaveRole()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);
        var role = AddUserWithRole("targetRole", "Analyst", 30, mockContext);

        // Act
        var result = await _sut.DeleteRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnSuccess_WhenUserRoleExistsAndUserIsAdmin()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteRole(user.User, user.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldRoleBeNullIfLookingForIt_WhenUserRoleExistsAndUserIsAdmin()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        await _sut.DeleteRole(user.User, user.Role);

        // Assert
        var result = mockContext.UserRoles.SingleOrDefault(ur =>
            ur.UserId == user.User.UserId && ur.RoleId == user.Role.RoleId);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteRole(new User() { Username = "testUser" },
            new Role() { RoleType = RoleType.DataAdmin.ToString() });
        // Assert
        Assert.Equal(ApiResponseType.NotFound, result.Type);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.DeleteUser(null);

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var response = await _sut.DeleteUser(null);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, response.Type);
    }

    /*[Fact]
    public async Task DeleteUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1, mockContext);

        // Act
        var result = await _sut.DeleteUser(new User());

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }*/

    [Fact]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteUser(new User() { Username = "testUser" });
        // Assert
        Assert.Equal(ApiResponseType.NotFound, result.Type);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnSuccess_WhenAdminAndUserExist()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteUser(user.User);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnYouCanNotDeleteYourself_WhenAdminDeleteHimself()
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1, mockContext);
        var user = AddUserWithRole("target", "DataAdmin", 2, mockContext);

        // Act
        var result = await _sut.DeleteUser(new User() { Username = "admin" });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }
}