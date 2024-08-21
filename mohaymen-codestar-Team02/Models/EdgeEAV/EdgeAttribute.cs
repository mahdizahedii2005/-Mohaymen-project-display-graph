using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeAttribute
{
    public EdgeAttribute(string name, long edgeEntityId)
    {
        Name = name;
        EdgeEntityId = edgeEntityId;
    }
    [Key] public long Id { get; set; }
    public string Name { get; set; }
    public long EdgeEntityId { get; set; }
    [ForeignKey("EdgeEntityId")] public virtual EdgeEntity EdgeEntity { get; set; }
    public virtual ICollection<EdgeValue> EdgeValues { get; set; } = new List<EdgeValue>();
}