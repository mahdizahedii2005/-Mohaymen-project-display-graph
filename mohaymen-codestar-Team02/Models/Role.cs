namespace mohaymen_codestar_Team02.Models;

public class Role
{
    public long RoleId { get; set; } 
    public string Name { get; set; }
    public User User { get; set; }
    public long UserId { get; set; }
    public RoleType RoleType { get; set; }
}