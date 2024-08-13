using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public interface IAdminService
{
    Task<ServiceResponse<int>> Register1(User user, long password);

    Task<ServiceResponse<int>> Register(User user, long password);
    Task<ServiceResponse<string>> AddRole(User user, Role role);
    Task<ServiceResponse<string>> DeleteRole(User user,Role role);
    Task<ServiceResponse<string>> Regiser2(User user, long password);
    public Task<ServiceResponse<string>> AddRole1();



}