using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public interface IProfileService
{
    Task<ServiceResponse<string>> ChangePassword(string username, string password);
    Task<ServiceResponse<string>> Logout();
}