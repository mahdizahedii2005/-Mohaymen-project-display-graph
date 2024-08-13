using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class Role
{
    public long RoleId { get; set; } 
    public RoleType RoleType { get; set; }
    public HashSet<UserRole> UserRoles { get; set; }

}