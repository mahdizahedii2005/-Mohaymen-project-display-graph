using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class User
{
    [Key] public long UserId { get; init; }

    [Required] 
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string? Username { get; set; } = string.Empty;

    [Required] [StringLength(64)] public string FirstName { get; set; } = string.Empty;

    [Required] [StringLength(64)] public string LastName { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    //dont add normal pass
    [Required] [StringLength(256)] public byte[] Salt { get; set; }

    [Required] [StringLength(256)] public byte[] PasswordHash { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<DataGroup> DataSets { get; set; }
}