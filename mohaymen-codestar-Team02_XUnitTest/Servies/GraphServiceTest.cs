using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using NSubstitute;
using QuikGraph;

namespace mohaymen_codestar_Team02_XUnitTest.Servies;

public class GraphServiceTest
{
    private GraphService _sut;
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;

    public GraphServiceTest()
    {
        _vertexService = Substitute.For<IVertexService>();
        _edgeService = Substitute.For<IEdgeService>();
        _sut = new GraphService(_vertexService, _edgeService);
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
            new()
            {
                Id = "id1",
                Value = "value1"
            },
            new()
            {
                Id = "id2",
                Value = "value2"
            }
        };

        var expectedEdge = new List<Edge>()
        {
            new()
            {
                Id = "id1",
                Source = "id1",
                Target = "id2"
            }
        };

        var expected = (expectedVertex, expectedEdge);

        _vertexService.GetAllVertices(datasetName, vertexIdentifierFieldName).Returns(expectedVertex);
        _edgeService.GetAllEdges(datasetName, vertexIdentifierFieldName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName).Returns(expectedEdge);

        // Act
        var actual = _sut.GetGraph(datasetName, sourceEdgeIdentifierFieldName, destinationEdgeIdentifierFieldName,
            vertexIdentifierFieldName);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}