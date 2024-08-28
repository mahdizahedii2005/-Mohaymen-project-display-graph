using QuikGraph;

namespace mohaymen_codestar_Team02.Models;

public class Edge : IEdge<string>
{
    public string? Id { get; init; }
    public string? Source { get; set; }
    public string? Target { get; set; }
}