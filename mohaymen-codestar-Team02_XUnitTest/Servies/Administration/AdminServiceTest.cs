using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.Administration;

public class AdminServiceTests
{
    private readonly AdminService _sut;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly DataContext _mockContext;

    public AdminServiceTests()
    {
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new DataContext(options);
        _sut = new AdminService(_mockHttpContextAccessor, _mockContext);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", RoleType.Analyst, 1);
        // Act
        var response = await _sut.AddRole(null, null);
        // Assert
        Assert.Equal(ApiResponse.BadRequest, response.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", RoleType.Analyst, 1);
        // Act
        var result = await _sut.AddRole(null, null);
        // Assert
        Assert.Equal(ApiResponse.Forbidden, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        // Act
        var result = await _sut.AddRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Unauthorized, result.Type);
    }

    [Fact]
    public async Task AddRole_ShouldReturnSuccess_WhenCommenderIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", RoleType.SystemAdmin, 1);
        var user = AddUserWithRole("target", RoleType.DataAdmin, 2);
        var role = AddUserWithRole("fakeUser", RoleType.Analyst, 3);
        // Act
        var result = await _sut.AddRole(user.User, role.Role);
        // Assert
        Assert.Equal(ApiResponse.Success, result.Type);
    }
    [Fact]
    public async Task DeleteRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        // Act
        var result = await _sut.DeleteRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Unauthorized, result.Type);
    }
    [Fact]
    public async Task DeleteRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        // Act
        var response = await _sut.DeleteRole(null, null);
        // Assert
        Assert.Equal(ApiResponse.BadRequest, response.Type);
    }
    [Fact]
    public async Task DeleteRole_ShouldReturnForbidden_WhenCommenderIsNotSystemAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("fakeAdmin");
        AddUserWithRole("fakeAdmin", RoleType.Analyst, 1);
        // Act
        var result = await _sut.DeleteRole(null, null);
        // Assert
        Assert.Equal(ApiResponse.Forbidden, result.Type);
    }
    [Fact]
    public async Task DeleteRole_ShouldReturnNotFound_WhenUserRoleNotExists()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", RoleType.SystemAdmin, 1);
        // Act
        var result = await _sut.DeleteRole(new User(){Username = "lol"}, null);
        // Assert
        Assert.Equal(ApiResponse.NotFound, result.Type);
    }

    [Fact]
    public async Task DeleteRole_ShouldReturn_WhenUserRoleExistsAndUserDontHaveRole()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", RoleType.SystemAdmin, 1);
        var user = AddUserWithRole("target", RoleType.DataAdmin, 2);
        var role = AddUserWithRole("targetRole", RoleType.Analyst,30);
        // Act
        var result = await _sut.DeleteRole(user.User, role.Role);
        // Assert
        Assert.Equal(ApiResponse.NotFound, result.Type);
    }
    [Fact]
    public async Task DeleteRole_ShouldReturnSuccess_WhenUserRoleExistsAndUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("admin");
        AddUserWithRole("admin", RoleType.SystemAdmin, 1);
        var user = AddUserWithRole("target", RoleType.DataAdmin, 2);
        // Act
        var result = await _sut.DeleteRole(user.User, user.Role);
        // Assert
        Assert.Equal(ApiResponse.Success, result.Type);
    }


    private void FixTheReturnOfCookies(string returnThis)
    {
        var mockHttpContext = Substitute.For<HttpContext>();
        var mockRequest = Substitute.For<HttpRequest>();
        var mockCookies = Substitute.For<IRequestCookieCollection>();
        mockCookies["login"].Returns(returnThis);
        mockRequest.Cookies.Returns(mockCookies);
        mockHttpContext.Request.Returns(mockRequest);
        _mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);
    }

    private UserRole AddUserWithRole(string UserName, RoleType roleType, long id)
    {
        User user = new User()
            { Salt = Array.Empty<byte>(), PasswordHash = Array.Empty<byte>(), Username = UserName, UserId = id };
        Role role = new Role() { RoleType = roleType, RoleId = id };
        UserRole userRole = new UserRole() { UserId = user.UserId, RoleId = role.RoleId };
        _mockContext.Users.Add(user);
        _mockContext.Roles.Add(role);
        _mockContext.UserRoles.Add(userRole);
        _mockContext.SaveChanges();
        return new UserRole() { Role = role, User = user };
    }
}