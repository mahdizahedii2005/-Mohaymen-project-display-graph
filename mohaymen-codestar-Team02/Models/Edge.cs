using QuikGraph;

namespace mohaymen_codestar_Team02.Models;

public class Edge : IEdge<string>
{
    public string Id;
    public string Source { get; }
    public string Target { get; }
}