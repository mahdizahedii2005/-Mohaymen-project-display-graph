using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class Attribute
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int EntityId { get; set; }
    [ForeignKey("EntityId")]
    public Entity Entity { get; set; }
    public ICollection<Value> values { get; set; }
}