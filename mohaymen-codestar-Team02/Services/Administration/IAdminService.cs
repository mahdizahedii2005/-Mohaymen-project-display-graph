using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public interface IAdminService
{
    Task<ServiceResponse<int>> Register(User user, long password);
    Task<ServiceResponse<string>> AddRole(User user, int roleId);
    Task<ServiceResponse<string>> DeleteRole(User user, int roleId);

}