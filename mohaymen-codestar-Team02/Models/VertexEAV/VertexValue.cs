using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexValue
{
    public VertexValue(string stringValue, long vertexAttributeId, string objectId)
    {
        StringValue = stringValue;
        VertexAttributeId = vertexAttributeId;
        ObjectId = objectId;
    }

    [Key] public long Id { get; set; }
    public string StringValue { get; set; }

    public long VertexAttributeId { get; set; }
    [ForeignKey("VertexAttributeId")] public virtual VertexAttribute VertexAttribute { get; set; }
    public string ObjectId { get; set; }
}