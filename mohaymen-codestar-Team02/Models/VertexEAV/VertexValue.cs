using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public sealed class VertexValue
{
    [Key] public int Id { get; set; }
    public string StringValue { get; set; } = string.Empty;

    public int AttributeId { get; set; }
    [ForeignKey("AttributeId")] public EdgeAttribute EdgeAttribute { get; set; }

    public int EntityId { get; set; }
    [ForeignKey("EntityId")] public EdgeEntity EdgeEntity { get; set; }

    public int DataSetId { get; set; }
    [ForeignKey("DataSetId")] public DataSet DataSet { get; set; }
}