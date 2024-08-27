using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class UserRole
{
    public long UserId { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }

    public long RoleId { get; set; }
    [ForeignKey("RoleId")] public virtual Role Role { get; set; }
}