using mohaymen_codestar_Team02.Dto.Role;

namespace mohaymen_codestar_Team02.Dto.User;

public class GetUserDto
{
    //public long UserId { get; init; }

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public virtual ICollection<GetRoleDto> Roles { get; set; }
}