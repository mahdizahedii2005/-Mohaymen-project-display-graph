namespace mohaymen_codestar_Team02_XUnitTest.Servies.TokenService;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        _configuration.GetSection("AppSettings:Token").Value.Returns("SuperSecretKey");

        _sut = new TokenService(_configuration, _httpContextAccessor);
    }


    [Fact]
    public void GetUserNameFromToken_ShouldReturnUsername_WhenTokenIsValid()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "testUser")
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
            new Claim(ClaimTypes.Email, "testuser@example.com")
        }, "mock"));

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(claims);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var username = _sut.GetUserNameFromToken();

        // Assert
        Assert.Null(username);
    }
}
