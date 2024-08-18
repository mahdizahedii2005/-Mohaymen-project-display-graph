using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class User
{
    [Key] public long UserId { get; init; }

    [Required] [StringLength(64)] public string Username { get; set; } = string.Empty;

    [Required] [StringLength(64)] public string FirstName { get; set; } = string.Empty;

    [Required] [StringLength(64)] public string LastName { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    //dont add normal pass
    [Required] [StringLength(256)] public byte[] Salt { get; set; }

    [Required] [StringLength(256)] public byte[] PasswordHash { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<DataSet> DataSets { get; set; } = new List<DataSet>();
}