using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;
using AutoMapper;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.Administration;

public class AdminServiceTest
{
    private readonly AdminService _sut;
    private readonly ICookieService _cookieService;
    private readonly DataContext _mockContext;
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
            .UseInMemoryDatabase(databaseName: new Random().NextInt64() + Guid.NewGuid().ToString())
            .Options;
        _mockContext = new DataContext(options);
        _sut = new AdminService(_mockContext, _cookieService, _tokenService, _passwordService, _mapper); // پاس دادن Mapper به کلاس AdminService
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
    public async Task AddRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1);

        // Act
        var result = await _sut.AddRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnSuccess_WhenCommenderIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);
        var role = AddUserWithRole("fakeUser", "Analyst", 3);

        // Act
        var result = await _sut.AddRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
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
    public async Task DeleteRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", "Analyst", 1);

        // Act
        var result = await _sut.DeleteRole(null, null);

        // Assert
        Assert.Equal(ApiResponseType.Forbidden, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleNotExists()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        AddUserWithRole("lol", "SystemAdmin", 44);

        // Act
        var result = await _sut.DeleteRole(new User { Username = "lol" }, new Role() { RoleType = "invalidRole" });

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenUserRoleExistsAndUserDontHaveRole()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);
        var role = AddUserWithRole("targetRole", "Analyst", 30);

        // Act
        var result = await _sut.DeleteRole(user.User, role.Role);

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturnSuccess_WhenUserRoleExistsAndUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);

        // Act
        var result = await _sut.DeleteRole(user.User, user.Role);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldRoleBeNullIfLoockingForIt_WhenUserRoleExistsAndUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", "SystemAdmin", 1);
        var user = AddUserWithRole("target", "DataAdmin", 2);

        // Act
        await _sut.DeleteRole(user.User, user.Role);

        // Assert
        var result = _mockContext.UserRoles.SingleOrDefault(ur =>
            ur.UserId == user.User.UserId && ur.RoleId == user.Role.RoleId);
        Assert.Null(result);
    }

    private void FixTheReturnOfCookies(string returnThis)
    {
        _cookieService.GetCookieValue().Returns(returnThis);
        _tokenService.GetUserNameFromToken().Returns(returnThis);
    }

    private UserRole AddUserWithRole(string userName, string roleType, long id)
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
        _mockContext.Users.Add(user);
        _mockContext.Roles.Add(role);
        _mockContext.UserRoles.Add(userRole);
        _mockContext.SaveChanges();
        return new UserRole { Role = role, User = user };
    }
}
