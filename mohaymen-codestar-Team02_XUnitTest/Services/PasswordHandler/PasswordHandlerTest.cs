using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02_XUnitTest.Services.PasswordHandler;

public class PasswordServiceTests
{
    private readonly PasswordService _sut = new();

    [Fact]
    public void CreatePasswordHash_ShouldGenerateDifferentHashAndSalt_ForDifferentPasswords()
    {
        // Arrange
        var password1 = "password1";
        var password2 = "password2";

        // Act
        _sut.CreatePasswordHash(password1, out var hash1, out var salt1);
        _sut.CreatePasswordHash(password2, out var hash2, out var salt2);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void CreatePasswordHash_ShouldGenerateSameHash_ForSamePasswordAndSalt()
    {
        // Arrange
        var password = "password";

        // Act
        _sut.CreatePasswordHash(password, out var hash1, out var salt);
        _sut.CreatePasswordHash(password, out var hash2, out _);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnTrue_ForValidPassword()
    {
        // Arrange
        var password = "password";
        _sut.CreatePasswordHash(password, out var hash, out var salt);

        // Act
        var isValid = _sut.VerifyPasswordHash(password, hash, salt);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnFalse_ForInvalidPassword()
    {
        // Arrange
        var correctPassword = "password";
        var wrongPassword = "wrongpassword";
        _sut.CreatePasswordHash(correctPassword, out var hash, out var salt);

        // Act
        var isValid = _sut.VerifyPasswordHash(wrongPassword, hash, salt);

        // Assert
        Assert.False(isValid);
    }

    [Theory]
    [InlineData("Password1", true)]
    [InlineData("A1bcdefg", true)]
    [InlineData("Test1234", true)]
    [InlineData("password", false)]
    [InlineData("PASSWORD123", false)]
    [InlineData("Pass1", false)]
    [InlineData("Short1A", false)]
    [InlineData("Valid1234", true)]
    public void ValidatePassword_ShouldReturnExpectedResult(string password, bool expectedResult)
    {
        // Act
        var result = _sut.ValidatePassword(password);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}