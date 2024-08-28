using System.Security.Claims;
using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Services;

public interface ITokenService
{
    public string CreateToken(List<Claim> claims);
    public string? GetUserNameFromToken();
    public string? GetRolesFromToken();
}