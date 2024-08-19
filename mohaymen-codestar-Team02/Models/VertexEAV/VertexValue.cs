using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public sealed class VertexValue(string value, int attId, int entityId)
{
    [Key] public int Id { get; set; }
    public string StringValue { get; set; } = value;

    public int AttributeId { get; set; } = attId;
    [ForeignKey("AttributeId")] public EdgeAttribute EdgeAttribute { get; set; }

    public int EntityId { get; set; } = entityId;
    [ForeignKey("EntityId")] public EdgeEntity EdgeEntity { get; set; }
}