using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class GraphService : IGraphService
{
    public (List<Vertex> vertices, List<Edge<string>> edges) GetGraph(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        throw new NotImplementedException();
    }
}