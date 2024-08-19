using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;

namespace mohaymen_codestar_Team02.Models;

public class DataGroup(string name, long userId)
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = name;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;

    public virtual EdgeEntity Edge { get; set; }
    public virtual VertexEntity Vertex { get; set; }

    public long UserId { get; set; } = userId;

    [ForeignKey("UserId")] public virtual User? User { get; set; }
}