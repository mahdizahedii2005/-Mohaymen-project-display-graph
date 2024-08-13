
using Microsoft.AspNetCore.Http;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;
using NSubstitute;
using MockQueryable.NSubstitute;
using Xunit;

public class AdminServiceTests
{
    private readonly IAdminService _sut;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly DataContext _mockContext;

    public AdminServiceTests()
    {
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _mockContext = Substitute.For<DataContext>();
        
        _sut = new AdminService(_mockHttpContextAccessor, _mockContext);
    }

    [Fact]
    public void AddRole_ShouldReturnBadRequest_WhenAdminNotFound()
    {
        // Arrange
        
        var users = new List<User>().AsQueryable().BuildMockDbSet();
        _mockContext.Users.Returns(users);

        var mockHttpContext = Substitute.For<HttpContext>();
        var mockRequest = Substitute.For<HttpRequest>();
        var mockCookies = Substitute.For<IRequestCookieCollection>();

        mockCookies["userId"].Returns("1");
        mockRequest.Cookies.Returns(mockCookies);
        mockHttpContext.Request.Returns(mockRequest);
        _mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);

        // Act
        var result = _sut.AddRole(new User(), 1);

        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Result.Type);
    }

    [Fact]
    public void AddRole_ShouldReturnForbidden_WhenAdminIsNotSystemAdmin()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, UserRoles = new HashSet<UserRole> { new UserRole { Role = new Role { RoleType = RoleType.Analyst } } } }
        }.AsQueryable().BuildMockDbSet();
        _mockContext.Users.Returns(users);

        var mockHttpContext = Substitute.For<HttpContext>();
        var mockRequest = Substitute.For<HttpRequest>();
        var mockCookies = Substitute.For<IRequestCookieCollection>();

        mockCookies["userId"].Returns("1");
        mockRequest.Cookies.Returns(mockCookies);
        mockHttpContext.Request.Returns(mockRequest);
        _mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);

        // Act
        var result = _sut.AddRole(new User(), 1);

        // Assert
        Assert.Equal(ApiResponse.Forbidden, result.Result.Type);
    }
}
