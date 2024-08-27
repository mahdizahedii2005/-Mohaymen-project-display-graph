using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;

namespace mohaymen_codestar_Team02.Models;

public class DataGroup
{
    public DataGroup(string name, long userId)
    {
        Name = name;
        UserId = userId;
    }

    public DataGroup()
    {
    }

    [Key] public long DataGroupId { get; set; }

    public string Name { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    public virtual EdgeEntity EdgeEntity { get; set; }
    public virtual VertexEntity VertexEntity { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")] public virtual User? User { get; set; }
}