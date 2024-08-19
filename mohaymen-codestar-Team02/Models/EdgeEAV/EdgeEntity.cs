using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeEntity
{
    public EdgeEntity(string name, string dataSetId)
    {
        _name = name + "!Edge" + "!" + Guid.NewGuid() + "!";
        DataSetId = dataSetId;
    }

    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
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

    public string DataSetId { get; set; }
    [ForeignKey("DataSetId")] public virtual DataGroup? DataGroup { get; set; }
    public virtual ICollection<EdgeAttribute> Attributes { get; set; } = new List<EdgeAttribute>();
}