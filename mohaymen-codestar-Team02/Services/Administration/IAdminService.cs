using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services.Administration;

public interface IAdminService
{
    Task<ServiceResponse<int>> Register(User user, string password);
    Task<ServiceResponse<string>> ChangeRole(User user, RoleType newRoleType);
}