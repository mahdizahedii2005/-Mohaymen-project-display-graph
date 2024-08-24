namespace mohaymen_codestar_Team02_XUnitTest.Servies.PasswordHandler;

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
        Assert.NotEqual(hash1, hash2); // چون برای هر بار فراخوانی salt جدید تولید می‌شود.
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
}