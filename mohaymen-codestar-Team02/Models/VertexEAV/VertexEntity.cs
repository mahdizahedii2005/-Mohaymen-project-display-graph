using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexEntity
{
    public VertexEntity(string name, long dataSetId)
    {
        if (!name.Contains("!"))
        {
            _name = name + "!vertex" + "!" + Guid.NewGuid() + "!";
            DataSetId = dataSetId;
        }
        else
        {
            throw new ArgumentException("your name contain !");
        }
    }

    [Key] public long Id { get; set; }
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
        set
        {
            if (!value.Contains("!"))
            {
                _name = value + "!vertex" + "!" + Guid.NewGuid() + "!";
            }
        }
    }

    public long DataSetId { get; set; }
    [ForeignKey("DataSetId")] public virtual DataGroup DataGroup { get; set; }
    public virtual ICollection<VertexAttribute> Attributes { get; set; } = new List<VertexAttribute>();
}