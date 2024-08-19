using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeEntity
{
    public EdgeEntity(string name, int dataSetId)
    {
        _name = name + "!Edge" + "!" + Guid.NewGuid() + "!";
        DataSetId = dataSetId;
    }

    [Key] public int Id { get; set; }
    private string _name;

    public string Name
    {
        get
        {
            Regex regex = new Regex(@"^(.+?)!");
            Match match = regex.Match(_name);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
        set { _name = value + "!Edge" + "!" + Guid.NewGuid() + "!"; }
    }

    public int DataSetId { get; set; }
    [ForeignKey("DataSetId")] public virtual DataSet DataSet { get; set; }
    public virtual ICollection<EdgeAttribute> Attributes { get; set; } = new List<EdgeAttribute>();
}