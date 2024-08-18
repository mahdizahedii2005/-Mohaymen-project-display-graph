using mohaymen_codestar_Team02.Dto.Role;
using mohaymen_codestar_Team02.Dto.User;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public interface IAdminService
{
    Task<ServiceResponse<List<GetUserDto>>> GetAllUsers();
    Task<ServiceResponse<GetUserDto>> GetUserByUsername(string username);
    Task<ServiceResponse<GetUserDto>> Register(User user, string password);
    Task<ServiceResponse<GetUserDto>> DeleteUser(User user);
    Task<ServiceResponse<List<GetRoleDto>>> GetAllRoles();
    Task<ServiceResponse<User>> AddRole(User user, Role role);
    Task<ServiceResponse<User>> DeleteRole(User user, Role role);
}