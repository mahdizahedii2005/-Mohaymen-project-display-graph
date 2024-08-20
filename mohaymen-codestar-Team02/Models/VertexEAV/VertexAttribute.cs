using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexAttribute
{
    public VertexAttribute(string name, long vertexEntityId)
    {
        Name = name;
        VertexEntityId = vertexEntityId;
    }
    [Key] public long Id { get; set; }
    public string Name { get; set; } 
    public long VertexEntityId { get; set; }
    [ForeignKey("VertexEntityId")] public virtual VertexEntity VertexEntity { get; set; }
    public virtual ICollection<VertexValue> VertexValues { get; set; } 
}