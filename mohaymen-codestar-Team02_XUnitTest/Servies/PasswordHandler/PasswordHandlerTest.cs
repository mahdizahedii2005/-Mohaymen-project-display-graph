namespace mohaymen_codestar_Team02_XUnitTest.Servies.PasswordHandler;
using Xunit;
using mohaymen_codestar_Team02.Services.PasswordHandller;

public class PasswordServiceTests
{
    private readonly PasswordService _sut;

    public PasswordServiceTests()
    {
        _sut = new PasswordService();
    }

    [Fact]
    public void CreatePasswordHash_ShouldGenerateDifferentHashAndSalt_ForDifferentPasswords()
    {
        // Arrange
        string password1 = "password1";
        string password2 = "password2";

        // Act
        _sut.CreatePasswordHash(password1, out byte[] hash1, out byte[] salt1);
        _sut.CreatePasswordHash(password2, out byte[] hash2, out byte[] salt2);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void CreatePasswordHash_ShouldGenerateSameHash_ForSamePasswordAndSalt()
    {
        // Arrange
        string password = "password";

        // Act
        _sut.CreatePasswordHash(password, out byte[] hash1, out byte[] salt);
        _sut.CreatePasswordHash(password, out byte[] hash2, out _);

        // Assert
        Assert.NotEqual(hash1, hash2); // چون برای هر بار فراخوانی salt جدید تولید می‌شود.
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnTrue_ForValidPassword()
    {
        // Arrange
        string password = "password";
        _sut.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

        // Act
        bool isValid = _sut.VerifyPasswordHash(password, hash, salt);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnFalse_ForInvalidPassword()
    {
        // Arrange
        string correctPassword = "password";
        string wrongPassword = "wrongpassword";
        _sut.CreatePasswordHash(correctPassword, out byte[] hash, out byte[] salt);

        // Act
        bool isValid = _sut.VerifyPasswordHash(wrongPassword, hash, salt);

        // Assert
        Assert.False(isValid);
    }
}
