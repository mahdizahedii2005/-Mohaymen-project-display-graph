using QuikGraph;

namespace mohaymen_codestar_Team02.Models;

public class Edge : IEdge<string>
{
    public string Id { get; init; }
    public string Source { get; init; }
    public string Target { get; init; }
}