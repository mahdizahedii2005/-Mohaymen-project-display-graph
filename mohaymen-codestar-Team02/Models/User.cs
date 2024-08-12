namespace mohaymen_codestar_Team02.Models;

public class User
{
    public UserId UserId { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string HashPassword { get; set; }
    public List<Role> Roles { get; set; }
}