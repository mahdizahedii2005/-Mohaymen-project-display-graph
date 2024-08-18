using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class EdgeAttribute
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EntityId { get; set; }
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; }
    public virtual ICollection<EdgeValue> Values { get; set; } = new List<EdgeValue>();
}