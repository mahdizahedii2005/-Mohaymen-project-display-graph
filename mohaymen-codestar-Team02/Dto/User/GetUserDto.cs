using mohaymen_codestar_Team02.Dto.Role;

namespace mohaymen_codestar_Team02.Dto.User;

public class GetUserDto
{
    //public long UserId { get; init; }

    public string Username { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public virtual ICollection<GetRoleDto> Roles { get; init; }
}