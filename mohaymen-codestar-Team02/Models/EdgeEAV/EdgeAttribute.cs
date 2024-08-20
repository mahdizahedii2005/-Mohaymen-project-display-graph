using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeAttribute(string name, long entityId)
{
    [Key] public long Id { get; set; }
    public string Name { get; set; } = name;
    public long EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; }
    public virtual ICollection<EdgeValue> Values { get; set; } = new List<EdgeValue>();
}