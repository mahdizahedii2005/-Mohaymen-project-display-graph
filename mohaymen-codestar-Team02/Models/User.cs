namespace mohaymen_codestar_Team02.Models;

public class User
{
    public long UserId { get; init; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public List<Role> Roles { get; set; }
}