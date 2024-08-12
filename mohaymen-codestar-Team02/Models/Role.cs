namespace mohaymen_codestar_Team02.Models;

public class Role
{
    public RoleId RoleId { get; set; }

    public string Name { get; set; }
    public User User { get; set; }
    public UserId UserId { get; set; }
    public RoleType RoleType { get; set; }
}