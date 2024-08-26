using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.EdgeEAV;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services;

public class EdgeServiceTest
{
    private IEdgeService _sut;
    private DataContext _dataContext;
    private ServiceProvider _serviceProvider;

    public EdgeServiceTest()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new EdgeService(_serviceProvider);
    }

    [Fact]
    public void GetAllEdges_ShouldReturnAllEdges_WhenGivenCorrectDatasetAndIdentifiersName()
    {
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        string datasetName = "DataSet1";
        string vertexIdentifierFieldName = "CardID";
        string sourceEdgeIdentifierFieldName = "SourceAcount";
        string destinationEdgeIdentifierFieldName = "DestiantionAccount";

        var expected = new List<Edge>()
        {
            new Edge()
            {
                Id = "id1",
                Source = "id2",
                Target = "id3"
            },
            new Edge()
            {
                Id = "id2",
                Source = "id4",
                Target = "id5"
            }
        };

        var dataset = new DataGroup("Dataset1", 1)
        {
            Name = "DataSet1",
            EdgeEntity = new EdgeEntity("Transaction", 1)
            {
                EdgeAttributes = new List<EdgeAttribute>
                {
                    new EdgeAttribute("SourceAcount", 1)
                    {
                        EdgeValues = new List<EdgeValue>
                        {
                            new EdgeValue("value1", 1, "id1") { EdgeAttribute = new EdgeAttribute("SourceAcount", 1)},
                            new EdgeValue("value2", 1, "id2") { EdgeAttribute = new EdgeAttribute("SourceAcount", 1)},

                        }
                    },
                    new EdgeAttribute("DestiantionAccount", 2)
                    {
                        EdgeValues = new List<EdgeValue>
                        {
                            new EdgeValue("value3", 2, "id1") { EdgeAttribute = new EdgeAttribute("DestiantionAccount", 1)},
                            new EdgeValue("value3", 2, "id2") { EdgeAttribute = new EdgeAttribute("DestiantionAccount", 1)},
                        }
                    }
                }
            },
            VertexEntity = new VertexEntity("Account", 1)
            {
                VertexAttributes = new List<VertexAttribute>
                {
                    new VertexAttribute("CardID", 1)
                    {
                        VertexValues = new List<VertexValue>
                        {
                            new VertexValue("value1", 1, "id2"){ VertexAttribute = new VertexAttribute("CardID", 1)},
                            new VertexValue("value3", 1, "id3") { VertexAttribute = new VertexAttribute("CardID", 1)},
                            new VertexValue("value2", 1, "id4") { VertexAttribute = new VertexAttribute("CardID", 1)},
                            new VertexValue("value3", 1, "id5") { VertexAttribute = new VertexAttribute("CardID", 1)}
                        }
                    }
                }
            }
        };

        _dataContext.DataSets.Add(dataset);
        _dataContext.SaveChanges();

        
        // Act
        var actual = _sut.GetAllEdges(datasetName, vertexIdentifierFieldName, sourceEdgeIdentifierFieldName,
            destinationEdgeIdentifierFieldName);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}