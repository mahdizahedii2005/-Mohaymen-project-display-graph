using System.Text;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.ProfileService;
using NSubstitute;
using Microsoft.AspNetCore.Http;
using mohaymen_codestar_Team02.Services;

public class ProfileServiceTests
{
    private readonly ProfileService _sut;
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataContext _mockContext;

    public ProfileServiceTests()
    {
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new DataContext(options);
        _sut = new ProfileService(_httpContextAccessor, _mockContext, _cookieService, _tokenService);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenCookieIsEmpty()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns((string)null);

        // Act
        var result = await _sut.ChangePassword("newPassword");

        // Assert
        Assert.Equal(ApiResponse.Unauthorized, result.Type);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns("nonexistentUser");

        // Act
        var result = await _sut.ChangePassword("newPassword");

        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Type);
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
        Assert.Equal(ApiResponse.Success, result.Type);

        // Verify that the user's password was updated
        var updatedUser = _mockContext.Users.SingleOrDefault(u => u.UserId == user.UserId);
        Assert.NotEqual(oldPass, updatedUser.PasswordHash); // Password should be changed
    }

    [Fact]
    public void Logout_ShouldClearLoginCookie_WhenCookieExists()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseCookies = Substitute.For<IResponseCookies>();
        context.Response.Cookies.Returns(responseCookies);

        _httpContextAccessor.HttpContext.Returns(context);

        // Act
        var result = _sut.Logout();

        // Assert
        Assert.Equal(ApiResponse.Success, result.Type);
        //TODO

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