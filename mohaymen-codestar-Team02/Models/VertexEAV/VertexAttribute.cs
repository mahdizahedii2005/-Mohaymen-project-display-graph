using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexAttribute
{
    [Key] public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long EntityId { get; set; }
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; } 
    public virtual ICollection<EdgeValue> Values { get; set; } = new List<EdgeValue>();
}