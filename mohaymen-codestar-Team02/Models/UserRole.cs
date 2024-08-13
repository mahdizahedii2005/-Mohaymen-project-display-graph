using mohaymen_codestar_Team02.Models;

public class UserRole
{
    public long UserId { get; set; }
    public virtual User User { get; set; }

    public long RoleId { get; set; }
    public virtual Role Role { get; set; }
    
    
}