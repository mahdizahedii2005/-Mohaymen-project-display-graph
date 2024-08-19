using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeValue(string value, int attId, int entityId)
{
    [Key] public int Id { get; set; }
    public string StringValue { get; set; } = value;

    public int AttributeId { get; set; } = attId;
    [ForeignKey("AttributeId")] public virtual EdgeAttribute EdgeAttribute { get; set; }

    public int EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; }
}