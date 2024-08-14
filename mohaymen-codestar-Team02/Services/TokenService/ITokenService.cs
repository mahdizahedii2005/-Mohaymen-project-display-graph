using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface ITokenService
{
    string CreateToken(User user);
    string CheckAccess(string token);

}