using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace mohaymen_codestar_Team02.Models.EdgeEAV;

public class EdgeEntity
{
    public EdgeEntity(string name, long dataGroupId)
    {
        Regex regex =
            new Regex(
                "^[^!]+!Edge![0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}!$");
        var match = regex.Match(name);
        if (match.Success)
        {
            _name = name;
            DataGroupId = dataGroupId;
        }
        else if (!name.Contains("!"))
        {
            _name = name + "!Edge" + "!" + Guid.NewGuid() + "!";
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
        set => _name = value + "!Edge" + "!" + Guid.NewGuid() + "!";
    }

    public long DataGroupId { get; set; }
    [ForeignKey("DataGroupId")] public virtual DataGroup? DataGroup { get; set; }
    public virtual ICollection<EdgeAttribute> EdgeAttributes { get; set; } = new List<EdgeAttribute>();
}