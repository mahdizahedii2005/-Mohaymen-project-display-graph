using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeValue(string stringValue, long attributeId, string objectId)
{
    [Key] public long Id { get; set; }
    public string StringValue { get; set; } = stringValue;
    public long AttributeId { get; set; } = attributeId;
    [ForeignKey("AttributeId")] public virtual EdgeAttribute EdgeAttribute { get; set; }
    public string ObjectId { get; set; } = objectId;
}