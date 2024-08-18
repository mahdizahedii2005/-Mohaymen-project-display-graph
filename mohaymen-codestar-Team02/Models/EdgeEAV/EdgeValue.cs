using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class EdgeValue
{
    [Key] public int Id { get; set; }
    public string StringValue { get; set; } = string.Empty;

    public int AttributeId { get; set; }
    [ForeignKey("AttributeId")] public virtual EdgeAttribute EdgeAttribute { get; set; }

    public int EntityId { get; set; }
    [ForeignKey("EntityId")] public virtual EdgeEntity EdgeEntity { get; set; }

    public int DataSetId { get; set; }
    [ForeignKey("DataSetId")] public virtual DataSet DataSet { get; set; }
}