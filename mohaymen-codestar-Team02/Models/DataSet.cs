using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;

public class DataSet(string name, int userId)
{
    [Key] public int Id { get; set; }

    public string Name { get; set; } = name;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;

    public virtual EdgeEntity Edge { get; set; }
    public virtual VertexEntity Vertex { get; set; }

    public int UserId { get; set; } = userId;

    [ForeignKey("UserId")] public virtual User? User { get; set; }
}