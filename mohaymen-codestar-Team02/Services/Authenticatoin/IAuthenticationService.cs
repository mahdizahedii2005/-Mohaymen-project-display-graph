using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public interface IAuthenticationService
{
    Task<ServiceResponse<string>> Login(string username, long password);
}