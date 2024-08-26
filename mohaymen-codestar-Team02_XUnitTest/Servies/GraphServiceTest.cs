using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;

namespace mohaymen_codestar_Team02_XUnitTest.Servies;

public class GraphServiceTest
{
    private IGraphService _sut;
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;

    public GraphServiceTest(IVertexService vertexService, IEdgeService edgeService)
    {
        _vertexService = vertexService;
        _edgeService = edgeService;
        _sut = new GraphService(vertexService, edgeService);
    }

    [Fact]
    public void GetGraph_ShouldReturnListOfVerticesAndDestinations_WhenGivenDatasetNameAndIdentifiers()
    {
        // Arrange
        var datasetName = "datasetName1";
        var sourceEdgeIdentifierFieldName = "AccountID";
        var destinationEdgeIdentifierFieldName = "SourceAcount";
        var vertexIdentifierFieldName = "DestiantionAccount";

        var expectedVertex = new List<Vertex>()
        {
            new Vertex(){
                Id = "id1",
                Value = "value1"
            },
            new Vertex(){
                Id = "id2",
                Value = "value2"
            }
        };

        var expectedEdge = new List<Edge>()
        {
            new Edge(){
                Id = "id1",
                Source = "id1",
                Target = "id2"
            }
        };

        // Act
        var actual = _sut.GetGraph(datasetName, sourceEdgeIdentifierFieldName, destinationEdgeIdentifierFieldName,
            vertexIdentifierFieldName);
        var actualVertex = actual.vertices;
        var actualEdge = actual.edges;

        // Assert
        Assert.Equivalent(expectedVertex, actualVertex);
        Assert.Equivalent(expectedEdge, actualEdge);
    }
}