using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.ProfileService;

public class ProfileServiceTests
{
    private readonly mohaymen_codestar_Team02.Services.ProfileService.ProfileService _sut;
    private readonly ICookieService _cookieService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _mockContext;
    private readonly IPasswordService _passwordService;

    public ProfileServiceTests()
    {
        _passwordService = Substitute.For<IPasswordService>();
        _cookieService = Substitute.For<ICookieService>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new DataContext(options);
        _sut = new mohaymen_codestar_Team02.Services.ProfileService.ProfileService(_httpContextAccessor, _mockContext, _cookieService, _passwordService);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenCookieIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns((string)null);

        // Act
        var result = await _sut.ChangePassword("newPassword");

        // Assert
        Assert.Equal(ApiResponseType.Unauthorized, result.Type);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns("nonexistentUser");

        // Act
        var result = await _sut.ChangePassword("newPassword");

        // Assert
        Assert.Equal(ApiResponseType.BadRequest, result.Type);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnSuccess_WhenPasswordIsChanged()
    {
        // Arrange
        var user = AddUserToDatabase("existingUser", "oldPassword");
        _cookieService.GetCookieValue().Returns("existingUser");
        var oldPass = user.PasswordHash;
        // Act
        var result = await _sut.ChangePassword("newPassword");

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);

        // Verify that the user's password was updated
        var updatedUser = _mockContext.Users.SingleOrDefault(u => u.UserId == user.UserId);
        Assert.NotEqual(oldPass, updatedUser.PasswordHash); // Password should be changed
    }

    [Fact]
    public void Logout_ShouldClearLoginCookie_WhenCookieExists()
    {
        // Arrange
        var context = Substitute.For<HttpContext>();
    
        // Mock response cookies
        var responseCookies = Substitute.For<IResponseCookies>();
    
        // Set mock response cookies in the context
        context.Response.Cookies.Returns(responseCookies);

        // Mock HttpContext and its request cookies
        var requestCookies = Substitute.For<IRequestCookieCollection>();
        requestCookies.ContainsKey("login").Returns(true);  // Ensure the cookie "login" exists

        context.Request.Cookies.Returns(requestCookies);
        _httpContextAccessor.HttpContext.Returns(context);

        // Act
        var result = _sut.Logout();

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
    
        // Verify that the "login" cookie was cleared
        responseCookies.Received(1).Append("login", "", Arg.Is<CookieOptions>(options => options.Expires < DateTime.Now));
    }

    private User AddUserToDatabase(string username, string password)
    {
        var user = new User
        {
            Username = username,
            Salt = new byte[128],
            PasswordHash = new byte[64],
        };

        using (var hmac = new System.Security.Cryptography.HMACSHA512(user.Salt))
        {
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        _mockContext.Users.Add(user);
        _mockContext.SaveChanges();
        return user;
    }
}