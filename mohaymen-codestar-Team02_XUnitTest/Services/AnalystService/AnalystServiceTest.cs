using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Dto;
using mohaymen_codestar_Team02.Dto.GraphDTO;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using NSubstitute;

namespace mohaymen_codestar_Team02_XUnitTest.Services.AnalystService;

public class AnalystServiceTest
{
    private readonly mohaymen_codestar_Team02.Services.AnalystService.AnalystService _sut;
    private readonly IVertexService _vertexService;
    private readonly IEdgeService _edgeService;
    private readonly IGraphService _graphService;

    public AnalystServiceTest()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => Substitute.For<DataContext>());
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _vertexService = Substitute.For<IVertexService>();
        _edgeService = Substitute.For<IEdgeService>();
        _graphService = Substitute.For<IGraphService>();

        _sut = new mohaymen_codestar_Team02.Services.AnalystService.AnalystService(serviceProvider, _vertexService, _edgeService, _graphService);
    }

    [Fact]
    public async Task DisplayGeraphData_ShouldReturnCorrectData_WhenDataIsValid()
    {
        // Arrange
        var databaseId = 1L;
        var sourceField = "sourceId";
        var destinationField = "targetId";
        var vertexField = "vertexId";
        var vertexAttributes = new Dictionary<string, string> { { "attr1", "value1" } };
        var edgeAttributes = new Dictionary<string, string> { { "attr1", "value1" } };

        var vertices = new Dictionary<string, Dictionary<string, string>>
        {
            { "vertex1", new Dictionary<string, string> { { "attr1", "value1" } } }
        };

        var edges = new Dictionary<string, Dictionary<string, string>>
        {
            { "edge1", new Dictionary<string, string> { { "source", "vertex1" }, { "target", "vertex2" } } }
        };

        var graph = new DisplayGraphDto
        {
            Vertices = new List<Vertex> { new Vertex { Id = "vertex1", Label = "Label1" } },
            Edges = new List<Edge> { new Edge { Id = "edge1", Source = "vertex1", Target = "vertex2" } }
        };

        _vertexService.GetAllVertices(databaseId, vertexField, vertexAttributes)
            .Returns(vertices);

        _edgeService.GetAllEdges(databaseId, sourceField, destinationField, edgeAttributes)
            .Returns(edges);

        _graphService.GetGraph(vertices, edges, vertexField, sourceField, destinationField)
            .Returns((graph.Vertices, graph.Edges));

        // Act
        var result = await _sut.DisplayGeraphData(databaseId, sourceField, destinationField, vertexField, vertexAttributes, edgeAttributes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Vertices);
        Assert.Equal("vertex1", result.Data.Vertices.First().Id);
        Assert.Single(result.Data.Edges);
        Assert.Equal("vertex1", result.Data.Edges.First().Source);
    }

    [Fact]
    public void GetVertexAttributes_ShouldReturnCorrectAttributes()
    {
        // Arrange
        var vertexEntityId = 1L;
        var attributes = new List<GetAttributeDto>
        {
            new GetAttributeDto { Id = 1, Name = "Attribute1" }
        };

        _vertexService.GetVertexAttributes(vertexEntityId)
            .Returns(attributes);

        // Act
        var result = _sut.GetVertexAttributes(vertexEntityId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Attribute1", result.Data.First().Name);
    }

    [Fact]
    public void GetEdgeAttributes_ShouldReturnCorrectAttributes()
    {
        // Arrange
        var edgeEntityId = 1L;
        var attributes = new List<GetAttributeDto>
        {
            new GetAttributeDto { Id = 1, Name = "Attribute1" }
        };

        _edgeService.GetEdgeAttributes(edgeEntityId)
            .Returns(attributes);

        // Act
        var result = _sut.GetEdgeAttributes(edgeEntityId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ApiResponseType.Success, result.Type);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Attribute1", result.Data.First().Name);
    }
}