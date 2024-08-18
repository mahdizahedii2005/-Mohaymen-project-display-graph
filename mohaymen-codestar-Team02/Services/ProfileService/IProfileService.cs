using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.ProfileService;

public interface IProfileService
{
    Task<ServiceResponse<GetUserDto?>> ChangePassword(string previousPassword, string newPassword);
    Task<ServiceResponse<GetUserDto?>> UpdateUser(UpdateUserDto updateUserDto);
}