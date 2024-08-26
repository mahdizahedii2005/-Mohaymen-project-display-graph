using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public interface IGraphService
{
    (List<Vertex> vertices, List<Edge> edges) GetGraph(string databaseName,
        string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName);
}