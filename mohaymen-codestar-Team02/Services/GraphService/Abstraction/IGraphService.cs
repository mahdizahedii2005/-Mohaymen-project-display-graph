using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public interface IGraphService
{
    public (List<Vertex> vertices, List<Edge> edges) GetGraph(Dictionary<string, Dictionary<string, string>> vertices,
        Dictionary<string, Dictionary<string, string>> edges, string vertexIdentifierFieldName,
        string SourceIdentifierFieldName, string TargetIdentifierFieldName);
}