namespace mohaymen_codestar_Team02.Services.PasswordHandller;

public interface IPasswordService
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    bool ValidatePassword(string password);
}