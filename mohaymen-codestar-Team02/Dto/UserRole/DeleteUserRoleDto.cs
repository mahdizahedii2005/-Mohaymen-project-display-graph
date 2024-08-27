using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Dto.UserRole;

public class DeleteUserRoleDto
{
    [Required] public string RoleType { get; set; }
}