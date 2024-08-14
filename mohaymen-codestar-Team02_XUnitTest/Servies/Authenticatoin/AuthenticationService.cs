using System.Text;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.CookieService;
using NSubstitute;
using System.Threading.Tasks;
using mohaymen_codestar_Team02.Services;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _sut;
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;
    private readonly DataContext _mockContext;

    public AuthenticationServiceTests()
    {
        _cookieService = Substitute.For<ICookieService>();
        _tokenService = Substitute.For<ITokenService>();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new DataContext(options);
        _sut = new AuthenticationService(_mockContext, _cookieService, _tokenService);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_ShouldReturnBadRequest_WhenUserIsNullOrEmpty(string username)
    {
        // Arrange
        string password = "password123";
        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        string username = "password123";
        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Type);
    }
    [Fact]
    public async Task Login_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        string username = "nonexistentUser";
        string password = "password123";

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponse.NotFound, result.Type);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        string username = "existingUser";
        string password = "wrongPassword";
        AddUserToDatabase(username, "correctPassword");

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponse.BadRequest, result.Type);
    }

    [Fact]
    public async Task Login_ShouldReturnSuccess_WhenCredentialsAreCorrect()
    {
        // Arrange
        string username = "existingUser";
        string password = "correctPassword";
        AddUserToDatabase(username, password);

        string fakeToken = "fakeToken";
        _tokenService.CreateToken(Arg.Any<User>()).Returns(fakeToken);

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.Equal(ApiResponse.Success, result.Type);
    }

    private void AddUserToDatabase(string username, string password)
    {
        var user = new User
        {
            Username = username,
            Salt = new byte[128], // Assuming 128-byte salt
            PasswordHash = new byte[64], // Assuming 64-byte hash
        };
        using (var hmac = new System.Security.Cryptography.HMACSHA512(user.Salt))
        {
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        _mockContext.Users.Add(user);
        _mockContext.SaveChanges();
    }
}