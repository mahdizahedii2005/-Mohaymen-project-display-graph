using mohaymen_codestar_Team02.Models;
using QuikGraph;

namespace mohaymen_codestar_Team02.Services;

public class GraphService : IGraphService
{
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;

    public GraphService(IVertexService vertexService, IEdgeService edgeService)
    {
        _vertexService = vertexService;
        _edgeService = edgeService;
    }

    public (List<Vertex> vertices, List<Edge> edges) GetGraph(string databaseName, string sourceEdgeIdentifierFieldName,
        string destinationEdgeIdentifierFieldName, string vertexIdentifierFieldName)
    {
        var vertices = _vertexService.GetAllVertices(databaseName, vertexIdentifierFieldName);
        var edges = _edgeService.GetAllEdges(databaseName, vertexIdentifierFieldName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName);
        return (vertices, edges);
    }
    
}