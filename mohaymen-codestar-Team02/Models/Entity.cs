using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models;

public class Entity
{
    [Key]
    public int Id { get; set; }
    private string Name { get; set; }
    
    public ICollection<Attribute> Attributes { get; set; }
}