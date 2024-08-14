using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models;

public class UserRole
{
    public long UserId { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }

    public long RoleId { get; set; }
    [ForeignKey("RoleId")] public virtual Role Role { get; set; }
}