using mohaymen_codestar_Team02.Models;

namespace mohaymen_codestar_Team02.Dto.GraphDTO;

public class DisplayGraphDto
{
    public List<Edge>? Edges { get; init; }
    public List<Vertex>? Vertices { get; init; }
    public long? GraphId { get; set; }
}