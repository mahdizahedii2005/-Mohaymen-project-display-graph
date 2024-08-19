using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexEntity
{
    [Key] public int Id { get; set; }
    public int DataSetId { get; set; }
    [ForeignKey("DataSetId")] public virtual DataSet DataSet { get; set; }
    private string Name { get; set; } = string.Empty;
    public virtual ICollection<EdgeAttribute> Attributes { get; set; } = new List<EdgeAttribute>();
}