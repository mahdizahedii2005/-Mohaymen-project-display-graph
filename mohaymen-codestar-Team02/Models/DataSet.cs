using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mohaymen_codestar_Team02.Models;

[Index(nameof(Name), IsUnique = true)]
public class DataSet
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreateAt;
    public DateTime UpdateAt;

    public virtual EdgeEntity Edge { get; set; }
    public virtual EdgeEntity Vertex { get; set; }

    public int CreatorId { get; set; }
    [ForeignKey("UserId")] public virtual User Creator { get; set; }

    // todo
}