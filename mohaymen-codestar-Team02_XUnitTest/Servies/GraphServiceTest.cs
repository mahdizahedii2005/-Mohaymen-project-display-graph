using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using NSubstitute;


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
        long datasetId = 1;
        string vertexAttName1 = "VertexAttName1";
        string vertexAttName2 = "VertexAttName2";
        string edgeAttName1 = "EdgeAttName1";
        string edgeAttName2 = "EdgeAttName2";
        string edgeAttName3 = "EdgeAttName3";
        var SourceIdentifierFieldName = edgeAttName1;
        var TargetIdentifierFieldName = edgeAttName2;
        var vertexIdentifierFieldName = vertexAttName1;

        var vertexAttributeValues = new Dictionary<string, Dictionary<string, string>>()
        {
            {"id1", new Dictionary<string, string>()
            {
                {vertexAttName1, "val1"},
                {vertexAttName2, "val2"},
            }},
            {"id2", new Dictionary<string, string>()
            {
                {vertexAttName1, "val1"},
                {vertexAttName2, "val3"},
            }},
            {"id3", new Dictionary<string, string>()
            {
                {vertexAttName1, "val2"},
                {vertexAttName2, "val2"},
            }}
        };
        
        var edgeAttributeValues = new Dictionary<string, Dictionary<string, string>>()
        {
            {"id1", new Dictionary<string, string>()
            {
                {edgeAttName1, "val1"},  // s= 1, 2    d= 3
                {edgeAttName2, "val2"}, 
                {edgeAttName3, "val8"}
            }},
            {"id2", new Dictionary<string, string>()
            {
                {edgeAttName1, "val2"}, // 
                {edgeAttName2, "val4"},
                {edgeAttName3, "val8"}
            }},
            {"id3", new Dictionary<string, string>()
            {
                {edgeAttName1, "val2"}, // s= 3   d= 1, 2
                {edgeAttName2, "val1"},
                {edgeAttName3, "val8"}
            }}
        };

        var expectedVertex = new List<Vertex>()
        {
            new()
            {
                Id = "id1",
                Label = "val1"
            },
            new()
            {
                Id = "id2",
                Label = "val1"
            },
            new()
            {
                Id = "id3",
                Label = "val2"
            }
        };
        
        var expectedEdge = new List<Edge>()
        {
            new()
            {
                Id = "id1",
                Source = "id1",
                Target = "id3"
            },
            new()
            {
                Id = "id1",
                Source = "id2",
                Target = "id3"
            },
            new()
            {
                Id = "id3",
                Source = "id3",
                Target = "id1"
            },
            new()
            {
                Id = "id3",
                Source = "id3",
                Target = "id2"
            }
        };
        
        var expected = (expectedVertex, expectedEdge);
        
        // Act
        var actual = _sut.GetGraph(vertexAttributeValues, edgeAttributeValues, vertexIdentifierFieldName, SourceIdentifierFieldName, TargetIdentifierFieldName);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}