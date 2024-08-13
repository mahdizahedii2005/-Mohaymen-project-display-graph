using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;
using NSubstitute;
using MockQueryable.NSubstitute;

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
    public void AddRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        FixTheReturnOfCookies("1");
        var user = new User { UserId = 2, Username = "TestUser" };
        _mockContext.Users.Add(user);
        _mockContext.SaveChangesAsync();

        // Act
        var response = _sut.AddRole(user, null);

        // Assert
        Assert.Equal(ApiResponse.BadRequest, response.Result.Type);
    }

    [Fact]
    public void AddRole_ShouldReturnForbidden_WhenAdminIsNotSystemAdmin()
    {
        // Arrange
        var user = new User
        {
            PasswordHash = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            UserId = 1,
            UserRoles = new HashSet<UserRole> { new UserRole { Role = new Role { RoleType = RoleType.Analyst } } }
        };
        _mockContext.Add(user);
        _mockContext.SaveChanges();
        FixTheReturnOfCookies("1");
        // Act
        var result = _sut.AddRole(user, null);

        // Assert
        Assert.Equal(ApiResponse.Forbidden, result.Result.Type);
    }

    [Fact]
    public void AddRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        // Act
        var result = _sut.AddRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Unauthorized, result.Result.Type);
    }

    [Fact]
    public void AddRole_ShouldReturnSuccess_WhenUserIsAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("1");
        UserRole userRole = new UserRole()
        {
            Role = new Role() { RoleType = RoleType.SystemAdmin },
            User = new User() { UserId = 1 }
        };
        User admin = new User()
        {
            PasswordHash = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            UserRoles = new HashSet<UserRole>() { userRole },
            UserId = 1
        };
        _mockContext.Users.Add(admin);
        _mockContext.SaveChanges();
        // Act
        var result = _sut.AddRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Success, result.Result.Type);
    }

    [Fact]
    public void DeletRole_ShouldReturnUnauthorized_WhenCookiesIsEmpty()
    {
        // Arrange
        // Act
        var result = _sut.DeleteRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Unauthorized, result.Result.Type);
    }

    [Fact]
    public void DeletRole_ShouldReturnBadRequest_WhenTheCookiesDataAreNotCorrect()
    {
        // Arrange
        FixTheReturnOfCookies("1");
        // Act
        var result = _sut.DeleteRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Result.Type);
    }

    [Fact]
    public void DeletRole_ShouldReturnForbiden_WhenTheCommenderIsNotAdmin()
    {
        // Arrange
        FixTheReturnOfCookies("1");
        User fakeAdmin = new User()
        {
            PasswordHash = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            UserId = 1
        };
        UserRole userRole = new UserRole()
        {
            UserId = 1,
            User = fakeAdmin,
            Role = new Role() { RoleType = RoleType.Analyst }
        };
        _mockContext.UserRoles.Add(userRole);
        _mockContext.SaveChanges();
        // Act
        var result = _sut.DeleteRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.Forbidden, result.Result.Type);
    }

    [Fact]
    public void DeletRole_ShouldReturnNotFound_WhenTheCommenderUSerNotFound()
    {
        // Arrange
        User admin = new User()
        {
            PasswordHash = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            UserId = 1
        };
        UserRole userRole = new UserRole()
        {
            Role = new Role() { RoleType = RoleType.SystemAdmin },
            UserId = 1,
            User = admin
        };
        FixTheReturnOfCookies("1");
        _mockContext.UserRoles.Add(userRole);
        _mockContext.SaveChanges();
        // Act
        var result = _sut.DeleteRole(new User(), null);
        // Assert
        Assert.Equal(ApiResponse.NotFound, result.Result.Type);
    }

    [Fact]
    public void DeletRole_ShouldReturnSuccess_WhenTheCommenderUserIsValid()
    {
        // Arrange
        UserRole user = new UserRole()
        {
            User = new User(){UserId = 2},
            Role = new Role(){RoleId = 2,RoleType = RoleType.Analyst}
        };
        User admin = new User()
        {
            PasswordHash = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            UserId = 1
        };
        UserRole AdminUserRole = new UserRole()
        {
            Role = new Role() { RoleType = RoleType.SystemAdmin },
            UserId = 1,
            User = admin
        };
        FixTheReturnOfCookies("1");
        _mockContext.UserRoles.Add(user);
        _mockContext.UserRoles.Add(AdminUserRole);
        _mockContext.SaveChanges();
        // Act
        var result = _sut.DeleteRole(user.User, user.Role);
        // Assert
        Assert.Equal(ApiResponse.Success, result.Result.Type);
    }

    private void FixTheReturnOfCookies(string returnThis)
    {
        var mockHttpContext = Substitute.For<HttpContext>();
        var mockRequest = Substitute.For<HttpRequest>();
        var mockCookies = Substitute.For<IRequestCookieCollection>();
        mockCookies["userId"].Returns(returnThis);
        mockRequest.Cookies.Returns(mockCookies);
        mockHttpContext.Request.Returns(mockRequest);
        _mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);
    } 
}