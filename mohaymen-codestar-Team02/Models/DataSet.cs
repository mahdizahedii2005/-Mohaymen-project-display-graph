using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class DataSet
{
    [Key]
    public int Id { get; set; }

    public DateTime CreateAt;
    public DateTime UpdateAt;
    
    public Entity Edge { get; set; }
    public Entity Vertex { get; set; }
    
    public int CreatorId { get; set; }
    [ForeignKey("UserId")]
    public User Creator { get; set; }
    
    // todo
}