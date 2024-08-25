namespace mohaymen_codestar_Team02_XUnitTest.Servies.TokenService;

using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;
using mohaymen_codestar_Team02.Services.TokenService;
using Microsoft.AspNetCore.Http;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenServiceTests()
    {
        _configuration = Substitute.For<IConfiguration>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        // Setup configuration
        _configuration.GetSection("AppSettings:Token").Value
            .Returns("SuperSuperSuperSuperSuperSuperSuperSuperSuperSuperSuperSuperSuperSuperSecretKey");

        _sut = new TokenService(_configuration, _httpContextAccessor);
    }

    [Fact]
    public void CreateToken_ValidClaims_ReturnsToken()
    {
        // Arrange
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.Role, "Admin")
        };

        // Act
        var token = _sut.CreateToken(claims);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void GetUserNameFromToken_ShouldReturnUsername_WhenTokenIsValid()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Name, "testUser")
        }, "mock"));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claims);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var username = _sut.GetUserNameFromToken();

        // Assert
        Assert.Equal("testUser", username);
    }

    [Fact]
    public void GetUserNameFromToken_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null);

        // Act
        var username = _sut.GetUserNameFromToken();

        // Assert
        Assert.Null(username);
    }

    [Fact]
    public void GetUserNameFromToken_ShouldReturnNull_WhenUserHasNoNameClaim()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Email, "testuser@example.com")
        }, "mock"));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claims);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var username = _sut.GetUserNameFromToken();

        // Assert
        Assert.Null(username);
    }

    [Fact]
    public void GetRolesFromToken_ShouldReturnUsername_WhenTokenIsValid()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Name, "testUser"),
            new(ClaimTypes.Role, "SystemAdmin")
        }, "mock"));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claims);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var role = _sut.GetRolesFromToken();

        // Assert
        Assert.Equal("SystemAdmin", role);
    }

    [Fact]
    public void GetRolesFromToken_ShouldReturnNull_WhenHttpContextIsNull()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null);

        // Act
        var role = _sut.GetRolesFromToken();

        // Assert
        Assert.Null(role);
    }

    [Fact]
    public void GetRolesFromToken_ShouldReturnNull_WhenUserHasNoRoleClaim()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Email, "testuser@example.com")
        }, "mock"));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claims);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var role = _sut.GetRolesFromToken();

        // Assert
        Assert.Null(role);
    }
}