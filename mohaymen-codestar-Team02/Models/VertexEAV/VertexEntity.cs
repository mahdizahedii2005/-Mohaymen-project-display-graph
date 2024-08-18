using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class VertexEntity
{
    [Key] public int Id { get; set; }
    private string Name { get; set; } = string.Empty;
    public virtual ICollection<EdgeAttribute> Attributes { get; set; } = new List<EdgeAttribute>();
}