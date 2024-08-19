using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexAttribute(string name, string entityId)
{
    [Key] public string Id { get; set; }= Guid.NewGuid().ToString();
    public string Name { get; set; } = name;
    public string EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public virtual VertexEntity EdgeEntity { get; set; }
    public virtual ICollection<VertexValue> Values { get; set; } 
}