using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using mohaymen_codestar_Team02.Models.EdgeEAV;

namespace mohaymen_codestar_Team02.Models.VertexEAV;

public class VertexEntity
{
    public VertexEntity() {}
    public VertexEntity(string name, long dataGroupId)
    {
        var regex =
            new Regex(
                "^[^!]+!vertex![0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}!$");
        var match = regex.Match(name);
        if (match.Success)
        {
            _name = name;
            DataGroupId = dataGroupId;
        }
        else if (!name.Contains("!"))
        {
            _name = name + "!vertex" + "!" + Guid.NewGuid() + "!";
            DataGroupId = dataGroupId;
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
            var regex = new Regex(@"^(.+?)!");
            var match = regex.Match(_name);
            if (match.Success) return match.Groups[1].Value;

            return null;
        }
        set
        {
            if (!value.Contains("!")) _name = value + "!vertex" + "!" + Guid.NewGuid() + "!";
        }
    }

    public long DataGroupId { get; set; }
    [ForeignKey("DataGroupId")] public virtual DataGroup DataGroup { get; set; }
    public virtual ICollection<VertexAttribute> VertexAttributes { get; set; } = new List<VertexAttribute>();
}