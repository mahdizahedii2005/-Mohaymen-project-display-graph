using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public interface IProfileService
{
    Task<ServiceResponse<User>> ChangePassword(string previousPassword , string newPassword);
    ServiceResponse<User> Logout();
    Task<ServiceResponse<User>> UpdateUser(UpdateUserDto updateUserDto);
}