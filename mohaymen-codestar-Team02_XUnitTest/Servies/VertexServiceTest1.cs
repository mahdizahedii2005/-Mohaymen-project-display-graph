using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Models.VertexEAV;
using mohaymen_codestar_Team02.Services;

namespace mohaymen_codestar_Team02_XUnitTest.Servies;

public class VertexServiceTest
{
    private IVertexService _sut;
    private DataContext _dataContext;
    private ServiceProvider _serviceProvider;

    public VertexServiceTest()
    { 
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new DataContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new VertexService(_serviceProvider);
    }

    [Fact]
    public void GetAllVertices_ShouldReturnAllVertices_WhenGivenCorrectDatasetName()
    {
        using var scope = _serviceProvider.CreateScope();
        _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Arrange
        string datasetName = "DataSet1";
        string vertexIdentifierFieldName = "CardID";

        var dataset = new DataGroup(datasetName, 1)
        {
            VertexEntity = new VertexEntity("Account", 1)
            {
                VertexAttributes = new List<VertexAttribute>
                {
                    new VertexAttribute("CardID", 1)
                    {
                        VertexValues = new List<VertexValue>
                        {
                            new VertexValue("value1", 1, "id1"){ VertexAttribute = new VertexAttribute("CardID", 1)},
                            new VertexValue("value2", 1, "id2") { VertexAttribute = new VertexAttribute("CardID", 1)}
                        }
                    }
                }
            }
        };

        _dataContext.DataSets.Add(dataset);
        _dataContext.SaveChanges();
        
        List<Vertex> expected = new List<Vertex>()
        {
            new Vertex()
            {
                Id = "id1",
                Value = "value1"
            }, 
            new Vertex()
            {
                Id = "id2",
                Value = "value2"
            }
        };
        

        // Act
        var actual = _sut.GetAllVertices(datasetName, vertexIdentifierFieldName);

        // Assert
        Assert.Equivalent(expected, actual);
    }
}