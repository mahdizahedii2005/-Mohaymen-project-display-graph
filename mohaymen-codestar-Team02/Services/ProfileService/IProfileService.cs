using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public interface IProfileService
{
    Task<ServiceResponse<User>> ChangePassword(string password);
    ServiceResponse<User> Logout();
    Task<ServiceResponse<User>> UpdateUser(User newUser);
}