using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeAttribute(string name,string entityId)
{
    [Key] public string Id { get; set; }= Guid.NewGuid().ToString();
    public string Name { get; set; } = name;
    public string EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; }
    public virtual ICollection<EdgeValue> Values { get; set; } = new List<EdgeValue>();
}