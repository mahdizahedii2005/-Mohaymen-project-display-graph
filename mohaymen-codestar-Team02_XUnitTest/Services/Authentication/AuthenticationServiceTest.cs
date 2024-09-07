using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Services.Authentication;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _sut;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IPasswordService _passwordService;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationServiceTests()
    {
        _passwordService = Substitute.For<IPasswordService>();
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();

        var config = new MapperConfiguration(cfg => { cfg.CreateMap<User, GetUserDto>(); });
        var mapper = config.CreateMapper();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => new DataContext(options));
        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new AuthenticationService(_serviceProvider, _cookieService, _tokenService, _passwordService, mapper);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_ShouldReturnBadRequest_WhenUserIsNullOrEmpty(string username)
    {
        // Arrange
        var password = "password123";

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        var username = "username123";

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistentUser";
        var password = "password123";

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var username = "existingUser";
        var password = "wrongPassword";
        AddUserToDatabase(username, "correctPassword");

        _passwordService.VerifyPasswordHash(password, Arg.Any<byte[]>(), Arg.Any<byte[]>())
            .Returns(false);

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task Login_ShouldReturnSuccess_WhenCredentialsAreCorrect()
    {
        // Arrange
        var username = "existingUser";
        var password = "correctPassword";
        AddUserToDatabase(username, password);

        _passwordService.VerifyPasswordHash(password, Arg.Any<byte[]>(), Arg.Any<byte[]>())
            .Returns(true);

        var fakeToken = "fakeToken";
        _tokenService.CreateToken(Arg.Any<List<Claim>>()).Returns(fakeToken);

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    private void AddUserToDatabase(string username, string password)
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var user = new User
        {
            Username = username,
            Salt = new byte[128],
            PasswordHash = new byte[64]
        };
        using (var hmac = new System.Security.Cryptography.HMACSHA512(user.Salt))
        {
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        mockContext.Users.Add(user);
        mockContext.SaveChanges();
    }

    [Fact]
    public void Logout_ShouldCallGetExpiredCookie_WhenCookieIsPresent()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns("someCookieValue");
        _cookieService.GetExpiredCookie();

        // Act
        var result = _sut.Logout();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public void Logout_ShouldNotCallGetExpiredCookie_WhenCookieIsNotPresent()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns((string?)null);
        _cookieService.DidNotReceive().GetExpiredCookie();
        // Act
        var result = _sut.Logout();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task GetPermission_ShouldReturnUnauthorized_WhenTokenIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns(string.Empty);

        // Act
        var result = await _sut.GetPermission();

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task GetPermission_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        FixTheReturnOfCookies("admin");

        // Act
        var result = await _sut.GetPermission();

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task GetPermission_ShouldReturnSuccess_WhenUserExist()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 3, new List<Permission>());

        // Act
        var result = await _sut.GetPermission();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }


    [Fact]
    public async Task GetPermission_ShouldReturnUnionPermissions_WhenUserExist()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        var systemAdminPermission = new List<Permission>
        {
            Permission.Login,
            Permission.Logout,
            Permission.UserRegister
        };
        AddUserWithRole("admin", "SystemAdmin", 4, systemAdminPermission);
        var dataAdminPermission = new List<Permission>
        {
            Permission.Login,
            Permission.Logout
        };
        AddUserWithRole("admin", "DataAdmin", 5, dataAdminPermission);
        _tokenService.GetRolesFromToken().Returns("SystemAdmin,DataAdmin");

        var expected = new List<Permission>
        {
            Permission.Login,
            Permission.Logout,
            Permission.UserRegister
        };

        // Act
        var result = await _sut.GetPermission();

        // Assert
        Assert.Equal(expected.Count, result.Data?.Permissions.Count);
    }


    private void FixTheReturnOfCookies(string? returnThis)
    {
        _cookieService.GetCookieValue().Returns(returnThis);
        _tokenService.GetUserNameFromToken().Returns(returnThis);
    }

    private void AddUserWithRole(string userName, string roleType, long id, List<Permission> permissions)
    {
        using var scope = _serviceProvider.CreateScope();
        var mockContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var user = new User
        {
            Salt = Array.Empty<byte>(),
            PasswordHash = Array.Empty<byte>(),
            Username = userName,
            UserId = id
        };

        var role = new Role { RoleType = roleType, RoleId = id, Permissions = permissions };
        var userRole = new UserRole { UserId = user.UserId, RoleId = role.RoleId };

        mockContext.Users.Add(user);
        mockContext.Roles.Add(role);
        mockContext.UserRoles.Add(userRole);
        mockContext.SaveChanges();
    }
}