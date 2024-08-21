using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Servies.Authenticatoin;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _sut;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly DataContext _mockContext;
    private readonly IPasswordService _passwordService;

    public AuthenticationServiceTests()
    {
        _passwordService = Substitute.For<IPasswordService>();
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();

        var config = new MapperConfiguration(cfg => { cfg.CreateMap<User, GetUserDto>(); });
        var mapper = config.CreateMapper();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _mockContext = new DataContext(options);
        _sut = new AuthenticationService(_mockContext, _cookieService, _tokenService, _passwordService, mapper);
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
        Assert.Null(result.Data);
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
        Assert.Null(result.Data);
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
        Assert.Null(result.Data);
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
        _tokenService.CreateToken(Arg.Any<Claim[]>()).Returns(fakeToken);

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.NotNull(result.Data);
    }

    private void AddUserToDatabase(string username, string password)
    {
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

        _mockContext.Users.Add(user);
        _mockContext.SaveChanges();
    }

    [Fact]
    public void Logout_ShouldCallGetExpiredCookie_WhenCookieIsPresent()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns("someCookieValue");

        // Act
        var result = _sut.Logout();

        // Assert
        _cookieService.Received(1).GetExpiredCookie();
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Logout_ShouldNotCallGetExpiredCookie_WhenCookieIsNotPresent()
    {
        // Arrange
        _cookieService.GetCookieValue().Returns((string?)null);

        // Act
        var result = _sut.Logout();

        // Assert
        _cookieService.DidNotReceive().GetExpiredCookie();
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.Null(result.Data);
    }
}