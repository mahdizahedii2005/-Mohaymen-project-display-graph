using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mohaymen_codestar_Team02.Models;

public class Value
{
    [Key]
    public int Id { get; set; }
    public string ValueString { get; set; }

    public int AttributeId { get; set; }
    [ForeignKey("AttributeId")]
    public Attribute Attribute { get; set; }
    
    public int EntityId { get; set; }
    [ForeignKey("EntityId")]
    public Entity Entity { get; set; }

    
    public int DataSetId { get; set; }
    [ForeignKey("DataSetId")]
    public DataSet DataSet { get; set; }
}