using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexAttribute(string name, long entityId)
{
    [Key] public long Id { get; set; }
    public string Name { get; set; } = name;
    public long EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public virtual VertexEntity EdgeEntity { get; set; }
    public virtual ICollection<VertexValue> Values { get; set; }
}