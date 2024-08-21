using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeValue
{
    public EdgeValue(string stringValue, long edgeAttributeId, string objectId)
    {
        StringValue = stringValue;
        EdgeAttributeId = edgeAttributeId;
        ObjectId = objectId;
    }
    [Key] public long Id { get; set; }
    public string StringValue { get; set; }
    public long EdgeAttributeId { get; set; }
    [ForeignKey("EdgeAttributeId")] public virtual EdgeAttribute EdgeAttribute { get; set; }
    public string ObjectId { get; set; }
}