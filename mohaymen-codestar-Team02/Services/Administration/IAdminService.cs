using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public interface IAdminService
{
    Task<ServiceResponse<int>> RegisterUser(User user, string password);

    Task<ServiceResponse<User>> Register(User user, string password);
    Task<ServiceResponse<User>> AddRole(User user, Role role);
    Task<ServiceResponse<User>> DeleteRole(User user, Role role);
    Task<ServiceResponse<string>> RegisterRoleTest(User user, string password);
    public Task<ServiceResponse<string>> AddRoleTest();
}