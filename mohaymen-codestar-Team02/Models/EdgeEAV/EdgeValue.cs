using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeValue(string stringValue, string attributeId, string objectId)
{
    [Key] public string Id { get; set; }= Guid.NewGuid().ToString();
    public string StringValue { get; set; } = stringValue;
    public string AttributeId { get; set; } = attributeId;
    [ForeignKey("AttributeId")] public virtual EdgeAttribute EdgeAttribute { get; set; }
    public string ObjectId { get; set; } = objectId;
}