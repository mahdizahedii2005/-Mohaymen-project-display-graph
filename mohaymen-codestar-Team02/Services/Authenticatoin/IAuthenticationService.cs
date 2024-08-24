using mohaymen_codestar_Team02.Dto.Permission;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Authenticatoin;

public interface IAuthenticationService
{
    Task<ServiceResponse<GetUserDto?>> Login(string username, string password);
    ServiceResponse<string?> Logout();
    Task<ServiceResponse<GetPermissionDto>> GetPermission();
}