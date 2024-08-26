using System.ComponentModel.DataAnnotations;

namespace mohaymen_codestar_Team02.Models.newModel;

public class Tables
{
    [Key] public long Id { get; set; }
    public string Name { get; set; }

}