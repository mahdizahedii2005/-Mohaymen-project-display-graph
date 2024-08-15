using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class Role
{
    [Key] public long RoleId { get; set; }
    [Required] public string RoleType { get; set; } = string.Empty;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}